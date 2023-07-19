using UnityEditor;

using Zlitz.AI.BehaviourTrees.Runtime;

namespace Zlitz.AI.BehaviourTrees.Editor
{
    [CustomEditor(typeof(BehaviourTreeNodeDecorator), true)]
    public class BehaviourTreeNodeDecoratorEditor : UnityEditor.Editor
    {
        private static readonly string[] s_excludedProperties = { 
            "m_guid",
            "m_Script"
        };

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
            {
                string guid = (target as BehaviourTreeNodeDecorator).guid;
                EditorGUILayout.TextField("GUID", guid);

                string type = BehaviourTreeNodeDecorators.GetBehaviourTreeNodeDecoratorTypeName(target.GetType());
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
