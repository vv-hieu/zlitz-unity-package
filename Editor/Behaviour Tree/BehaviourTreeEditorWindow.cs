using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using Zlitz.AI.BehaviourTrees.Runtime;

namespace Zlitz.AI.BehaviourTrees.Editor
{
    public class BehaviourTreeEditorWindow : EditorWindow
    {
        [SerializeField]
        private BehaviourTree m_behaviourTree;

        [SerializeField]
        private BehaviourTreeNode m_behaviourTreeNode;

        [SerializeField]
        private BehaviourTreeNodeDecorator m_behaviourTreeNodeDecorator;

        [SerializeField]
        private BehaviourTreeExecutor m_selectedObject;

        private SerializedObject m_serializedBehaviourTree;

        private SerializedProperty m_parentOfSelectedProperty;
        private int                m_selectedIndex;

        private List<Object>           m_createdObjects;
        private List<Object>           m_removedObjects;
        private List<SerializedObject> m_changedNodes;

        private Vector2 m_scrollPos1 = Vector2.zero;
        private Vector2 m_scrollPos2 = Vector2.zero;

        private float m_currentSplitViewWidth;
        private bool  m_resizingScrollView = false;
        private bool  m_movingNode         = false;
        private Rect  m_cursorChangeRect;

        private Dictionary<BehaviourTreeNode, UnityEditor.Editor>          m_nodeEditors;
        private Dictionary<BehaviourTreeNodeDecorator, UnityEditor.Editor> m_decoratorEditors;
        private Dictionary<BehaviourTreeNode, string>                      m_names;
        private bool                                                       m_selectedNode = false;

        private AdvancedDropdownState m_nodeTypesDropdownState;
        private AdvancedDropdownState m_decoratorTypesDropdownState;

        private static readonly string  s_saveChangesMessage      = "Unsaved changes detected. Would you like to save?";
        private static readonly Vector2 s_nodeSize                = new Vector2(200.0f, 80.0f);
        private static readonly Vector2 s_nodeSpacing             = new Vector2(30.0f, 40.0f);
        private static readonly Color   s_nodeColorDark           = new Color(0.15f, 0.15f, 0.15f, 1.0f);
        private static readonly Color   s_nodeColorLight          = new Color(0.2f, 0.2f, 0.2f, 1.0f);
        private static readonly Color   s_nodeColorBorder         = Color.white;
        private static readonly Color   s_nodeNameColorIdle       = Color.gray;
        private static readonly Color   s_nodeNameColorRunning    = Color.cyan;
        private static readonly Color   s_nodeNameColorSuccessful = Color.green;
        private static readonly Color   s_nodeNameColorFailed     = Color.red;
        private static readonly float   s_decoratorHeight         = 20.0f;
        private static readonly float   s_exposeButtonHeight      = 20.0f;

        [MenuItem("Zlitz/AI/Behaviour Trees/Behaviour Tree Editor")]
        public static void ShowWindow()
        {
            BehaviourTreeEditorWindow window = GetWindow<BehaviourTreeEditorWindow>("Behaviour Tree Editor");
            window.m_behaviourTree = null;
        }

        public static void ShowWindow(BehaviourTree behaviourTree)
        {
            BehaviourTreeEditorWindow window = GetWindow<BehaviourTreeEditorWindow>("Behaviour Tree Editor");
            window.m_behaviourTree = behaviourTree;
            window.OnBehaviourTreeChanged();
        }

        private void OnEnable()
        {
            m_currentSplitViewWidth = EditorPrefs.GetFloat("Zlitz.AI.BehaviourTrees.Editor.BehaviourTreeEditorWindow_splitViewWidth", 400.0f);
            m_cursorChangeRect      = new Rect(m_currentSplitViewWidth, 0.0f, 5.0f, position.height);

            saveChangesMessage = s_saveChangesMessage;

            m_createdObjects = new List<Object>();
            m_removedObjects = new List<Object>();
            m_changedNodes   = new List<SerializedObject>();

            m_nodeEditors      = new Dictionary<BehaviourTreeNode, UnityEditor.Editor>();
            m_decoratorEditors = new Dictionary<BehaviourTreeNodeDecorator, UnityEditor.Editor>();
            m_names            = new Dictionary<BehaviourTreeNode, string>();

            Repaint();
        }

        private void OnDisable()
        {
            if (m_nodeEditors != null)
            {
                foreach (UnityEditor.Editor editor in m_nodeEditors.Values)
                {
                    DestroyImmediate(editor);
                }
            }
        }

        private void OnGUI()
        {
            if (m_serializedBehaviourTree == null)
            {
                m_serializedBehaviourTree = m_behaviourTree == null ? null : new SerializedObject(m_behaviourTree);
            }

            m_cursorChangeRect = new Rect(m_currentSplitViewWidth, 0.0f, 5.0f, position.height);

            GUILayout.BeginHorizontal();

            m_scrollPos1 = GUILayout.BeginScrollView(m_scrollPos1, GUILayout.Width(m_currentSplitViewWidth));

            BehaviourTreePropertyGUI();
            BehaviourTreeNodePropertyGUI();
            BehaviourTreeNodeDecoratorPropertyGUI();

            GUILayout.EndScrollView();

            GUILayout.Space(5.0f);

            m_scrollPos2 = GUILayout.BeginScrollView(m_scrollPos2);

            BehaviourTreeView();

            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();

            ResizeSplitView();

            if (hasUnsavedChanges)
            {
                SaveChanges();
            }
        }

        private void OnSelectionChange()
        {
            if (Selection.activeGameObject != null && Selection.activeGameObject.TryGetComponent(out BehaviourTreeExecutor executor))
            {
                m_selectedObject = executor;
            }
            else
            {
                m_selectedObject = null;
            }
        }

        public override void SaveChanges()
        {
            m_serializedBehaviourTree.ApplyModifiedProperties();

            foreach (SerializedObject node in m_changedNodes)
            {
                if (node.targetObject != null)
                {
                    node.ApplyModifiedProperties();
                }
            }

            foreach (UnityEditor.Editor editor in m_nodeEditors.Values)
            {
                if (editor.serializedObject.targetObject != null)
                {
                    editor.serializedObject.ApplyModifiedProperties();
                }
            }

            foreach (UnityEditor.Editor editor in m_decoratorEditors.Values)
            {
                if (editor.serializedObject.targetObject != null)
                {
                    editor.serializedObject.ApplyModifiedProperties();
                }
            }

            while (m_removedObjects != null && m_removedObjects.Count > 0)
            {
                List<Object> newRemovedObjects = new List<Object>();

                foreach (Object removedObject in m_removedObjects)
                {
                    SerializedObject serializedObject = new SerializedObject(removedObject);
                    if (removedObject is BehaviourTreeNode node)
                    {
                        SerializedProperty decoratorsProperty = serializedObject.FindProperty("m_decorators");
                        for (int i = 0; i < decoratorsProperty.arraySize; i++)
                        {
                            newRemovedObjects.Add(decoratorsProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                        }

                        if (node is CompositeBehaviourTreeNode compositeNode)
                        {
                            SerializedProperty childrenProperty = serializedObject.FindProperty("m_children");
                            for (int i = 0; i < childrenProperty.arraySize; i++)
                            {
                                newRemovedObjects.Add(childrenProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                            }
                        }
                    }   

                    DestroyImmediate(removedObject, true);
                }

                m_removedObjects = newRemovedObjects;
            }

            foreach (var p in m_names)
            {
                if (p.Key != null)
                {
                    p.Key.name = p.Value;
                }
            }
            m_names.Clear();

            AssetDatabase.SaveAssets();

            ForceUpdateProjectWindows();

            base.SaveChanges();
        }

        public override void DiscardChanges()
        {
            foreach (Object createdObject in m_createdObjects)
            {
                DestroyImmediate(createdObject, true);
            }

            base.DiscardChanges();
        }

        private void BehaviourTreePropertyGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Behaviour Tree"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            SetBehaviourTree(EditorGUILayout.ObjectField(new GUIContent("Selected Tree"), m_behaviourTree, typeof(BehaviourTree), true) as BehaviourTree);
            
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        private void BehaviourTreeNodePropertyGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Behaviour Tree Node"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if (m_behaviourTreeNode == null)
            {
                EditorGUILayout.LabelField(new GUIContent("Select a node to edit."));
            }
            else
            {
                string currentName = GetNodeName(m_behaviourTreeNode);
                string newName     = EditorGUILayout.DelayedTextField(new GUIContent("Name"), currentName);
                if (currentName != newName)
                {
                    m_names[m_behaviourTreeNode] = newName;
                    hasUnsavedChanges = true;
                }

                GetNodeEditor(m_behaviourTreeNode).OnInspectorGUI();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        private void BehaviourTreeNodeDecoratorPropertyGUI()
        {
            EditorGUILayout.LabelField(new GUIContent("Behaviour Tree Node Decorator"), EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            if (m_behaviourTreeNodeDecorator == null)
            {
                EditorGUILayout.LabelField(new GUIContent("Select a node decorator to edit."));
            }
            else
            {
                GetDecoratorEditor(m_behaviourTreeNodeDecorator).OnInspectorGUI();
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        private void BehaviourTreeView()
        {
            if (m_serializedBehaviourTree == null)
            {
                return;
            }

            if (Event.current.type == EventType.MouseDown)
            {
                m_selectedNode = false;
            }

            SerializedProperty rootNodeProperty = m_serializedBehaviourTree.FindProperty("m_rootNode");

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal(GUILayout.Width(BehaviourTreeNodeWidth(rootNodeProperty.objectReferenceValue as BehaviourTreeNode) * (s_nodeSize.x + s_nodeSpacing.x) - s_nodeSpacing.x));
            GUILayout.FlexibleSpace();

            BehaviourTreeNodeView(rootNodeProperty, null, AddRootNode, RemoveRootNode, null, 0, 0, false);

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown && !m_selectedNode)
            {
                GUI.FocusControl(null);
                m_behaviourTreeNode = null;
                m_parentOfSelectedProperty = null;
                m_selectedIndex = -1;
                m_behaviourTreeNodeDecorator = null;
                Repaint();
            }
            if (Event.current.type == EventType.MouseUp)
            {
                m_movingNode = false;
            }
        }

        private void BehaviourTreeNodeView(SerializedProperty nodeProperty, SerializedProperty parentProperty, AddNodeFunction addNode, RemoveCallback onRemove, MoveNodeFunction moveNode, int index, int siblingsCount, bool isChildOfSelected, Vector2? origin = null, float? ry = null)
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(s_nodeSpacing.y);

            EditorGUILayout.BeginHorizontal();

            Rect containerRect;
            Rect rect;

            if (nodeProperty != null && nodeProperty.objectReferenceValue != null)
            {
                if (nodeProperty.objectReferenceValue == m_behaviourTreeNode)
                {
                    isChildOfSelected = true;
                }

                SerializedProperty decoratorsProperty = new SerializedObject(nodeProperty.objectReferenceValue).FindProperty("m_decorators");

                float height = s_nodeSize.y + s_decoratorHeight * (decoratorsProperty.arraySize + 1);
                if (nodeProperty.objectReferenceValue is CompositeBehaviourTreeNode)
                {
                    height += s_exposeButtonHeight;
                }

                containerRect = GUILayoutUtility.GetRect(s_nodeSize.x, height);
                rect = containerRect;
                float dx = Mathf.Max(rect.width, s_nodeSize.x) - s_nodeSize.x;
                rect.width = s_nodeSize.x;
                rect.x += dx * 0.5f;

                Handles.DrawSolidRectangleWithOutline(rect, s_nodeColorDark, nodeProperty.objectReferenceValue == m_behaviourTreeNode ? s_nodeColorBorder : s_nodeColorDark);

                if (origin.HasValue)
                {
                    float r = ry.HasValue ? 0.75f - Mathf.Abs(ry.Value - 0.5f) : 0.5f;

                    Vector2 p1 = origin.Value;
                    Vector2 p2 = rect.position + new Vector2(rect.width * 0.5f, 0.0f);
                    Vector2 p3 = new Vector2((p1.x + p2.x) * 0.5f, Mathf.Lerp(origin.Value.y, p2.y, r));
                    Vector2 t1 = new Vector2(p1.x, p3.y);
                    Vector2 t2 = new Vector2(p2.x, p3.y);

                    Handles.DrawBezier(p1, p3, t1, t1, s_nodeColorBorder, null, 2.0f);
                    Handles.DrawBezier(p2, p3, t2, t2, s_nodeColorBorder, null, 2.0f);
                }

                Color nodeNameColor = Color.white;
                if (Application.isPlaying && m_selectedObject != null && m_selectedObject.state != null)
                {
                    if (decoratorsProperty.arraySize > 0)
                    {
                        BehaviourTreeNodeDecoratorState decoratorState = m_selectedObject.state.GetDecoratorState(decoratorsProperty.GetArrayElementAtIndex(0).objectReferenceValue as BehaviourTreeNodeDecorator);
                        switch (decoratorState.state)
                        {
                            case BehaviourTreeNodeDecoratorState.State.Idle:
                                nodeNameColor = s_nodeNameColorIdle;
                                break;
                            case BehaviourTreeNodeDecoratorState.State.Running:
                                nodeNameColor = s_nodeNameColorRunning;
                                break;
                            case BehaviourTreeNodeDecoratorState.State.Successful:
                                nodeNameColor = s_nodeNameColorSuccessful;
                                break;
                            case BehaviourTreeNodeDecoratorState.State.Failed:
                                nodeNameColor = s_nodeNameColorFailed;
                                break;
                        }
                    }
                    else
                    {
                        BehaviourTreeNodeState nodeState = m_selectedObject.state.GetNodeState(nodeProperty.objectReferenceValue as BehaviourTreeNode);
                        switch (nodeState.state)
                        {
                            case BehaviourTreeNodeState.State.Idle:
                                nodeNameColor = s_nodeNameColorIdle;
                                break;
                            case BehaviourTreeNodeState.State.Running:
                                nodeNameColor = s_nodeNameColorRunning;
                                break;
                            case BehaviourTreeNodeState.State.Successful:
                                nodeNameColor = s_nodeNameColorSuccessful;
                                break;
                            case BehaviourTreeNodeState.State.Failed:
                                nodeNameColor = s_nodeNameColorFailed;
                                break;
                        }
                    }

                    Repaint();
                }

                bool mouseDownOnDecorator = false;

                Rect decoratorRect = rect;
                decoratorRect.height = s_decoratorHeight;

                GUIStyle decoIconStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                decoIconStyle.fontStyle = FontStyle.Bold;
                decoIconStyle.fontSize = 16;

                for (int i = decoratorsProperty.arraySize - 1; i >= 0; i--)
                {
                    SerializedProperty decoratorProperty = decoratorsProperty.GetArrayElementAtIndex(i);
                    if (decoratorProperty.objectReferenceValue == null)
                    {
                        decoratorsProperty.DeleteArrayElementAtIndex(i);
                    }
                    decoratorsProperty.serializedObject.ApplyModifiedProperties();
                }

                Rect decoRect = decoratorRect;
                for (int i = 0; i < decoratorsProperty.arraySize; i++)
                {
                    SerializedProperty decoratorProperty = decoratorsProperty.GetArrayElementAtIndex(i);
                    BehaviourTreeNodeDecorator decorator = decoratorProperty.objectReferenceValue as BehaviourTreeNodeDecorator;
                    
                    decoRect = decoratorRect;
                    decoRect.position += Vector2.one * Handles.lineThickness * 0.5f;
                    decoRect.size     -= Vector2.one * Handles.lineThickness;
                    Handles.DrawSolidRectangleWithOutline(decoRect, s_nodeColorLight, m_behaviourTreeNodeDecorator == decorator ? s_nodeColorBorder : s_nodeColorLight);

                    EditorGUI.LabelField(decoRect, new GUIContent(decorator.GetDecoratorDescription()));

                    if (m_behaviourTreeNode == nodeProperty.objectReferenceValue && Event.current.type == EventType.MouseDown && decoRect.Contains(Event.current.mousePosition))
                    {
                        m_behaviourTreeNodeDecorator = decorator;
                        mouseDownOnDecorator = true;
                        Repaint();
                    }

                    float decoRectRight = decoRect.xMax;
                    decoRect.width = decoRect.height;

                    decoRect.x = decoRectRight - decoRect.width;
                    if (GUI.Button(decoRect, "-", decoIconStyle))
                    {
                        m_removedObjects.Add(decorator);
                        decoratorProperty.objectReferenceValue = null;
                        hasUnsavedChanges = true;
                        Repaint();
                    }

                    decoRect.x = decoRectRight - decoRect.width * 2.0f;
                    if (GUI.Button(decoRect, EditorGUIUtility.IconContent("d_scrolldown_uielements@2x"), decoIconStyle) && i < decoratorsProperty.arraySize - 1)
                    {
                        SerializedProperty decoratorProperty2 = decoratorsProperty.GetArrayElementAtIndex(i + 1);
                        decoratorProperty.objectReferenceValue = decoratorProperty2.objectReferenceValue;
                        decoratorProperty2.objectReferenceValue = decorator;
                        decoratorsProperty.serializedObject.ApplyModifiedProperties();
                    }

                    decoRect.x = decoRectRight - decoRect.width * 3.0f;
                    if (GUI.Button(decoRect, EditorGUIUtility.IconContent("d_scrollup_uielements@2x"), decoIconStyle) && i > 0)
                    {
                        SerializedProperty decoratorProperty2 = decoratorsProperty.GetArrayElementAtIndex(i - 1);
                        decoratorProperty.objectReferenceValue = decoratorProperty2.objectReferenceValue;
                        decoratorProperty2.objectReferenceValue = decorator;
                        decoratorsProperty.serializedObject.ApplyModifiedProperties();
                    }

                    decoratorRect.y += s_decoratorHeight;
                }

                decoRect = decoratorRect;
                decoRect.position += Vector2.one * Handles.lineThickness * 0.5f;
                decoRect.size     -= Vector2.one * Handles.lineThickness;
                Handles.DrawSolidRectangleWithOutline(decoRect, s_nodeColorLight, s_nodeColorLight);

                EditorGUI.LabelField(decoRect, new GUIContent("+"), decoIconStyle);

                if (m_behaviourTreeNode == nodeProperty.objectReferenceValue && Event.current.type == EventType.MouseDown && decoRect.Contains(Event.current.mousePosition))
                {
                    m_decoratorTypesDropdownState = new AdvancedDropdownState();
                    BehaviourTreeNodeDecoratorTypeDropdown behaviourTreeNodeDecoratorTypeDropdown = new BehaviourTreeNodeDecoratorTypeDropdown(m_decoratorTypesDropdownState, (type) =>
                    {
                        int newDecoratorIndex = decoratorsProperty.arraySize++;
                        SerializedProperty newDecoratorProperty = decoratorsProperty.GetArrayElementAtIndex(newDecoratorIndex);

                        BehaviourTreeNodeDecorator decorator = CreateInstance(type) as BehaviourTreeNodeDecorator;
                        decorator.name = "Node Decorator";
                        AssetDatabase.AddObjectToAsset(decorator, m_behaviourTree);
                        m_createdObjects.Add(decorator);

                        GUI.FocusControl(null);

                        newDecoratorProperty.objectReferenceValue = decorator;
                        newDecoratorProperty.serializedObject.ApplyModifiedProperties();
                        hasUnsavedChanges = true;
                        Repaint();
                    });
                    behaviourTreeNodeDecoratorTypeDropdown.Show(new Rect(Event.current.mousePosition, new Vector2(200.0f, 0.0f)));
                    mouseDownOnDecorator = true;
                }

                Color restore = GUI.contentColor;
                GUI.contentColor = nodeNameColor;
                GUIStyle nodeNameStyle = new GUIStyle(EditorStyles.boldLabel);
                nodeNameStyle.alignment = TextAnchor.MiddleCenter;
                nodeNameStyle.wordWrap  = true;
                nodeNameStyle.fontSize  = 24;

                Rect nodeNameRect = rect;
                nodeNameRect.y += s_decoratorHeight * (decoratorsProperty.arraySize + 1);
                nodeNameRect.size = s_nodeSize;

                EditorGUI.LabelField(nodeNameRect, GetNodeName(nodeProperty.objectReferenceValue as BehaviourTreeNode), nodeNameStyle);
                GUI.contentColor = restore;

                bool mouseInRect = rect.Contains(Event.current.mousePosition);
                if (Event.current.type == EventType.MouseDown && mouseInRect)
                {
                    GUI.FocusControl(null);
                    m_behaviourTreeNode = nodeProperty.objectReferenceValue as BehaviourTreeNode;
                    m_parentOfSelectedProperty = parentProperty;
                    m_selectedIndex = index;
                    m_selectedNode = true;
                    Repaint();

                    if (!mouseDownOnDecorator)
                    {
                        m_movingNode = true;
                    }
                }
            }
            else
            {
                containerRect = GUILayoutUtility.GetRect(s_nodeSize.x, s_nodeSize.y);
                rect = containerRect;
                float dx = Mathf.Max(rect.width, s_nodeSize.x) - s_nodeSize.x;
                rect.width = s_nodeSize.x;
                rect.x += dx * 0.5f;

                if (origin.HasValue)
                {
                    float r = ry.HasValue ? 0.75f - Mathf.Abs(ry.Value - 0.5f) : 0.5f;

                    Vector2 p1 = origin.Value;
                    Vector2 p2 = rect.position + new Vector2(rect.width * 0.5f, 0.0f);
                    Vector2 p3 = new Vector2((p1.x + p2.x) * 0.5f, Mathf.Lerp(origin.Value.y, p2.y, r));
                    Vector2 t1 = new Vector2(p1.x, p3.y);
                    Vector2 t2 = new Vector2(p2.x, p3.y);

                    Handles.DrawBezier(p1, p3, t1, t1, s_nodeColorBorder, null, 2.0f);
                    Handles.DrawBezier(p2, p3, t2, t2, s_nodeColorBorder, null, 2.0f);
                }

                EditorGUI.DrawRect(rect, s_nodeColorLight);
                
                GUIStyle plusIconStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                plusIconStyle.fontSize  = 64;
                plusIconStyle.fontStyle = FontStyle.Bold;

                if (GUI.Button(rect, new GUIContent("+"), plusIconStyle))
                {
                    m_nodeTypesDropdownState = new AdvancedDropdownState();
                    BehaviourTreeNodeTypeDropdown nodeTypesDropdown = new BehaviourTreeNodeTypeDropdown(m_nodeTypesDropdownState, addNode);
                    nodeTypesDropdown.Show(new Rect(Event.current.mousePosition, new Vector2(200.0f, 0.0f)));
                }
            }

            EditorGUILayout.EndHorizontal();

            bool isRoot = (parentProperty == null);
            if (!isRoot)
            {
                Rect leftRect  = new Rect(rect.x - 0.5f * s_nodeSpacing.x, rect.y, s_nodeSpacing.x * 0.5f, rect.height);
                Rect rightRect = new Rect(rect.x + rect.width, rect.y, s_nodeSpacing.x * 0.5f, rect.height);

                bool mouseInLeftRect  = leftRect.Contains(Event.current.mousePosition);
                bool mouseInRightRect = rightRect.Contains(Event.current.mousePosition);

                leftRect.x += leftRect.width - s_nodeSpacing.x * 0.15f;
                leftRect.width  = s_nodeSpacing.x * 0.15f;
                rightRect.width = s_nodeSpacing.x * 0.15f;

                if (m_behaviourTreeNode != null && !isChildOfSelected)
                {
                    if (mouseInLeftRect)
                    {
                        EditorGUI.DrawRect(leftRect, Color.cyan);
                        Repaint();
                    }
                    if (mouseInRightRect)
                    {
                        EditorGUI.DrawRect(rightRect, Color.cyan);
                        Repaint();
                    }
                }

                if (Event.current.type == EventType.MouseUp)
                {
                    if (m_movingNode && m_behaviourTreeNode != null && !isChildOfSelected)
                    {
                        if (mouseInLeftRect)
                        {
                            moveNode?.Invoke(Mathf.Clamp(index, 0, siblingsCount));
                        }
                        if (mouseInRightRect)
                        {
                            moveNode?.Invoke(Mathf.Clamp(index + 1, 0, siblingsCount));
                        }
                    }
                }
            }

            if (nodeProperty != null && nodeProperty.objectReferenceValue != null && nodeProperty.objectReferenceValue is CompositeBehaviourTreeNode compositeNode)
            {
                SerializedProperty collapsedProperty = new SerializedObject(nodeProperty.objectReferenceValue).FindProperty("m_collapsed");

                Rect exposeButtonRect = rect;
                exposeButtonRect.y += exposeButtonRect.height - s_exposeButtonHeight;
                exposeButtonRect.height = s_exposeButtonHeight;
                exposeButtonRect.position += Vector2.one * Handles.lineThickness * 0.5f;
                exposeButtonRect.size -= Vector2.one * Handles.lineThickness;

                GUIStyle exposeIconStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                exposeIconStyle.fontStyle = FontStyle.Bold;
                exposeIconStyle.fontSize  = 16;

                Handles.DrawSolidRectangleWithOutline(exposeButtonRect, s_nodeColorLight, s_nodeColorDark);

                EditorGUI.LabelField(exposeButtonRect, new GUIContent("..."), exposeIconStyle);

                if (Event.current.type == EventType.MouseDown && exposeButtonRect.Contains(Event.current.mousePosition))
                {
                    collapsedProperty.boolValue = !collapsedProperty.boolValue;
                    collapsedProperty.serializedObject.ApplyModifiedProperties();
                }

                if (!collapsedProperty.boolValue)
                {
                    EditorGUILayout.BeginHorizontal();

                    SerializedProperty childrenProperty = new SerializedObject(compositeNode).FindProperty("m_children");

                    for (int i = childrenProperty.arraySize - 1; i >= 0; i--)
                    {
                        SerializedProperty childProperty = childrenProperty.GetArrayElementAtIndex(i);
                        if (childProperty.objectReferenceValue == null)
                        {
                            childrenProperty.DeleteArrayElementAtIndex(i);
                        }
                        childrenProperty.serializedObject.ApplyModifiedProperties();
                    }

                    float childSpacing = rect.width / (childrenProperty.arraySize + 2.0f);

                    for (int i = 0; i < childrenProperty.arraySize; i++)
                    {
                        Vector2 childOrigin = new Vector2(rect.x + (i + 1) * childSpacing, rect.y + rect.height);
                        float r = i / (float)childrenProperty.arraySize;

                        SerializedProperty childProperty = childrenProperty.GetArrayElementAtIndex(i);
                        BehaviourTreeNodeView(childProperty, nodeProperty, null, () => {
                            m_removedObjects.Add(childProperty.objectReferenceValue);
                            childProperty.objectReferenceValue = null;

                            GUI.FocusControl(null);
                            m_behaviourTreeNode = null;
                            m_parentOfSelectedProperty = null;
                            m_selectedIndex = -1;

                            childProperty.serializedObject.ApplyModifiedProperties();
                            hasUnsavedChanges = true;
                            Repaint();
                        }, (idx) => {
                            SerializedProperty siblingsProperty = new SerializedObject(m_parentOfSelectedProperty.objectReferenceValue).FindProperty("m_children");
                            SerializedProperty selectedProperty = siblingsProperty.GetArrayElementAtIndex(m_selectedIndex);
                            selectedProperty.objectReferenceValue = null;
                            selectedProperty.serializedObject.ApplyModifiedProperties();

                            SerializedProperty siblingsProperty2 = new SerializedObject(nodeProperty.objectReferenceValue).FindProperty("m_children");
                            siblingsProperty2.InsertArrayElementAtIndex(idx);
                            SerializedProperty selectedProperty2 = siblingsProperty2.GetArrayElementAtIndex(idx);
                            selectedProperty2.objectReferenceValue = m_behaviourTreeNode;
                            selectedProperty2.serializedObject.ApplyModifiedProperties();

                            hasUnsavedChanges = true;
                            Repaint();

                        }, i, childrenProperty.arraySize, isChildOfSelected, childOrigin, r);
                        GUILayout.Space(s_nodeSpacing.x);
                    }
                    BehaviourTreeNodeView(null, nodeProperty, (type) => {
                        int newChildIndex = childrenProperty.arraySize++;
                        SerializedProperty newChildProperty = childrenProperty.GetArrayElementAtIndex(newChildIndex);

                        BehaviourTreeNode node = CreateInstance(type) as BehaviourTreeNode;
                        node.name = "Node";
                        AssetDatabase.AddObjectToAsset(node, m_behaviourTree);
                        m_createdObjects.Add(node);

                        m_changedNodes.Add(childrenProperty.serializedObject);

                        GUI.FocusControl(null);
                        m_behaviourTreeNode = node;
                        m_parentOfSelectedProperty = parentProperty;
                        m_selectedIndex = index;

                        newChildProperty.objectReferenceValue = node;
                        newChildProperty.serializedObject.ApplyModifiedProperties();
                        hasUnsavedChanges = true;
                        Repaint();
                    }, null, (idx) => {
                        SerializedProperty siblingsProperty = new SerializedObject(m_parentOfSelectedProperty.objectReferenceValue).FindProperty("m_children");
                        SerializedProperty selectedProperty = siblingsProperty.GetArrayElementAtIndex(m_selectedIndex);
                        selectedProperty.objectReferenceValue = null;
                        selectedProperty.serializedObject.ApplyModifiedProperties();

                        SerializedProperty siblingsProperty2 = new SerializedObject(nodeProperty.objectReferenceValue).FindProperty("m_children");
                        siblingsProperty2.InsertArrayElementAtIndex(idx);
                        SerializedProperty selectedProperty2 = siblingsProperty2.GetArrayElementAtIndex(idx);
                        selectedProperty2.objectReferenceValue = m_behaviourTreeNode;
                        selectedProperty2.serializedObject.ApplyModifiedProperties();

                        hasUnsavedChanges = true;
                        Repaint();

                    }, childrenProperty.arraySize, childrenProperty.arraySize, isChildOfSelected, new Vector2(rect.x + (childrenProperty.arraySize + 1) * childSpacing, rect.y + rect.height), 1.0f);

                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndVertical();

            if (nodeProperty != null && nodeProperty.objectReferenceValue == m_behaviourTreeNode && m_behaviourTreeNode != null && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Delete)
            {
                onRemove?.Invoke();
            }
        }

        private int BehaviourTreeNodeWidth(BehaviourTreeNode node)
        {
            if (node != null)
            {
                int result = 1;

                if (node is CompositeBehaviourTreeNode compositeNode)
                {
                    SerializedProperty collapsedProperty = new SerializedObject(node).FindProperty("m_collapsed");
                    if (!collapsedProperty.boolValue)
                    {
                        SerializedProperty childrenProperty = new SerializedObject(compositeNode).FindProperty("m_children");
                        for (int i = 0; i < childrenProperty.arraySize; i++)
                        {
                            BehaviourTreeNode childNode = childrenProperty.GetArrayElementAtIndex(i).objectReferenceValue as BehaviourTreeNode;
                            result += BehaviourTreeNodeWidth(childNode);
                        }
                    }
                }
            }

            return 1;
        }

        private void AddRootNode(System.Type type)
        {
            if (m_behaviourTree != null)
            {
                SerializedProperty rootNodeProperty = m_serializedBehaviourTree.FindProperty("m_rootNode");

                BehaviourTreeNode root = CreateInstance(type) as BehaviourTreeNode;
                root.name = "Node";
                AssetDatabase.AddObjectToAsset(root, m_behaviourTree);
                m_createdObjects.Add(root);

                rootNodeProperty.objectReferenceValue = root;
                hasUnsavedChanges = true;
                m_behaviourTreeNode = root;
                m_parentOfSelectedProperty = null;
                m_selectedIndex = 0;
                Repaint();
            }
        }

        private void RemoveRootNode()
        {
            if (m_behaviourTree != null)
            {
                SerializedProperty rootNodeProperty = m_serializedBehaviourTree.FindProperty("m_rootNode");

                if (rootNodeProperty.objectReferenceValue != null)
                {
                    m_removedObjects.Add(rootNodeProperty.objectReferenceValue);
                    rootNodeProperty.objectReferenceValue = null;
                    rootNodeProperty.serializedObject.ApplyModifiedProperties();

                    GUI.FocusControl(null);
                    m_behaviourTreeNode = null;
                    m_parentOfSelectedProperty = null;
                    m_selectedIndex = -1;

                    hasUnsavedChanges = true;
                    Repaint();
                }
            }
        }

        private UnityEditor.Editor GetNodeEditor(BehaviourTreeNode node)
        {
            if (!m_nodeEditors.TryGetValue(node, out UnityEditor.Editor editor))
            {
                editor = UnityEditor.Editor.CreateEditor(node);
                m_nodeEditors.Add(node, editor);
            }
            return editor;
        }

        private UnityEditor.Editor GetDecoratorEditor(BehaviourTreeNodeDecorator decorator)
        {
            if (!m_decoratorEditors.TryGetValue(decorator, out UnityEditor.Editor editor))
            {
                editor = UnityEditor.Editor.CreateEditor(decorator);
                m_decoratorEditors.Add(decorator, editor);
            }
            return editor;
        }

        private string GetNodeName(BehaviourTreeNode node)
        {
            if (!m_names.TryGetValue(node, out string name))
            {
                name = node.name;
                m_names.Add(node, name);
            }
            return name;
        }

        private void ResizeSplitView()
        {
            GUI.DrawTexture(m_cursorChangeRect, EditorGUIUtility.whiteTexture);
            EditorGUIUtility.AddCursorRect(m_cursorChangeRect, MouseCursor.ResizeHorizontal);

            if (Event.current.type == EventType.MouseDown && m_cursorChangeRect.Contains(Event.current.mousePosition))
            {
                m_resizingScrollView = true;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                m_resizingScrollView = false;
            }

            if (m_resizingScrollView)
            {
                m_currentSplitViewWidth = Mathf.Clamp(Event.current.mousePosition.x, 0.01f * position.width, 0.99f * position.width);
                EditorPrefs.SetFloat("Zlitz.AI.BehaviourTrees.Editor.BehaviourTreeEditorWindow_splitViewWidth", m_currentSplitViewWidth);
                m_cursorChangeRect.Set(m_currentSplitViewWidth, m_cursorChangeRect.y, m_cursorChangeRect.width, m_cursorChangeRect.height);
                Repaint();
            }
        }

        private void SetBehaviourTree(BehaviourTree behaviourTree)
        {
            if (m_behaviourTree != behaviourTree)
            {
                if (hasUnsavedChanges)
                {
                    if (m_behaviourTree != null)
                    {
                        int action = EditorUtility.DisplayDialogComplex("", s_saveChangesMessage, "Save", "Cancel", "Discard");
                        switch (action)
                        {
                            case 0:
                                {
                                    SaveChanges();
                                    m_behaviourTree = behaviourTree;
                                    OnBehaviourTreeChanged();
                                    break;
                                }
                            case 2:
                                {
                                    DiscardChanges();
                                    m_behaviourTree = behaviourTree;
                                    OnBehaviourTreeChanged();
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
                else
                {
                    m_behaviourTree = behaviourTree;
                    OnBehaviourTreeChanged();
                }
            }
        }
        
        private void OnBehaviourTreeChanged()
        {
            GUI.FocusControl(null);

            m_serializedBehaviourTree = m_behaviourTree == null ? null : new SerializedObject(m_behaviourTree);
            m_serializedBehaviourTree?.Update();

            m_behaviourTreeNode = null;
            m_parentOfSelectedProperty = null;
            m_selectedIndex = -1;
        }

        private void ForceUpdateProjectWindows()
        {
            string tempAssetPath = "Assets/temp-987654321.anim";
            AssetDatabase.CreateAsset(new AnimationClip(), tempAssetPath);
            AssetDatabase.DeleteAsset(tempAssetPath);
        }

        private delegate void AddNodeFunction(System.Type nodeType);
        private delegate void MoveNodeFunction(int destIndex);
        private delegate void AddDecoratorFunction(System.Type decoratorType);
        private delegate void RemoveCallback();

        private class BehaviourTreeNodeTypeDropdown : AdvancedDropdown
        {
            private AddNodeFunction m_addNodeFunction;

            public BehaviourTreeNodeTypeDropdown(AdvancedDropdownState state, AddNodeFunction addNodeFunction) : base(state)
            {
                m_addNodeFunction = addNodeFunction;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                AdvancedDropdownItem root = new AdvancedDropdownItem("Node Types");

                AdvancedDropdownItem compositeNodes = new AdvancedDropdownItem("Composite");
                AdvancedDropdownItem actionNodes    = new AdvancedDropdownItem("Action");

                IDictionary<System.Type, string> nodeTypes = BehaviourTreeNodes.GetBehaviourTreeNodeTypes();

                foreach (var p in nodeTypes)
                {
                    if (p.Key.IsSubclassOf(typeof(CompositeBehaviourTreeNode)))
                    {
                        compositeNodes.AddChild(new AdvancedDropdownItem(p.Value));
                    }
                    else if (p.Key.IsSubclassOf(typeof(ActionBehaviourTreeNode))) 
                    {
                        actionNodes.AddChild(new AdvancedDropdownItem(p.Value));
                    }
                }

                root.AddChild(compositeNodes);
                root.AddChild(actionNodes);

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                System.Type nodeType = BehaviourTreeNodes.GetBehaviourTreeNodeType(item.name);
                m_addNodeFunction?.Invoke(nodeType);
            }
        }

        private class BehaviourTreeNodeDecoratorTypeDropdown : AdvancedDropdown
        {
            private AddDecoratorFunction m_addDecoratorFunction;

            public BehaviourTreeNodeDecoratorTypeDropdown(AdvancedDropdownState state, AddDecoratorFunction addDecoratorFunction) : base(state)
            {
                m_addDecoratorFunction = addDecoratorFunction;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                AdvancedDropdownItem root = new AdvancedDropdownItem("Node Decorator Types");

                IDictionary<System.Type, string> decoratorTypes = BehaviourTreeNodeDecorators.GetBehaviourTreeNodeDecoratorTypes();

                foreach (var p in decoratorTypes)
                {
                    root.AddChild(new AdvancedDropdownItem(p.Value));
                }

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                System.Type decoratorType = BehaviourTreeNodeDecorators.GetBehaviourTreeNodeDecoratorType(item.name);
                m_addDecoratorFunction?.Invoke(decoratorType);
            }
        }

    }
}
