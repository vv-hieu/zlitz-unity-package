using UnityEngine;
using UnityEditor;

using Zlitz.AI.BehaviourTrees.Runtime;

namespace Zlitz.AI.BehaviourTrees.Editor
{
    [CustomEditor(typeof(BehaviourTree))]
    public class BehaviourTreeEditor : UnityEditor.Editor
    {
        private SerializedProperty m_rootNodeProperty;

        private void OnEnable()
        {
            m_rootNodeProperty = serializedObject.FindProperty("m_rootNode");
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(m_rootNodeProperty);
            }
            if (m_rootNodeProperty.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("This behaviour tree doesn't contain any node and therefore will not run.", MessageType.Warning);
            }

            if (GUILayout.Button("Open Editor"))
            {
                BehaviourTreeEditorWindow.ShowWindow(serializedObject.targetObject as BehaviourTree);
            }
        }
    }
}
