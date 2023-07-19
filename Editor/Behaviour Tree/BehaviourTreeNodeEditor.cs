using System.IO;
using UnityEngine;
using UnityEditor;

using Zlitz.AI.BehaviourTrees.Runtime;

namespace Zlitz.AI.BehaviourTrees.Editor
{
    [CustomEditor(typeof(BehaviourTreeNode), true)]
    public class BehaviourTreeNodeEditor : UnityEditor.Editor
    {
        private static readonly string[] s_excludedProperties = { 
            "m_guid",
            "m_collapsed",
            "m_children",
            "m_decorators",
            "m_Script"
        };

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                string guid = (target as BehaviourTreeNode).guid;
                EditorGUILayout.TextField("GUID", guid);

                string type = BehaviourTreeNodes.GetBehaviourTreeNodeTypeName(target.GetType());
                EditorGUILayout.TextField("Type", type);
            }

            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, s_excludedProperties);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
