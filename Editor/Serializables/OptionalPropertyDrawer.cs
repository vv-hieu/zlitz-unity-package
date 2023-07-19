using UnityEditor;
using UnityEngine;

using Zlitz.Utility.Serializables.Runtime;

namespace Zlitz.Utility.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property.FindPropertyRelative("m_value"));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProperty   = property.FindPropertyRelative("m_value");
            SerializedProperty enabledProperty = property.FindPropertyRelative("m_enabled");

            EditorGUI.BeginProperty(position, label, property);

            position.width -= 24.0f;
            using (new EditorGUI.DisabledScope(!enabledProperty.boolValue))
            {
                EditorGUI.PropertyField(position, valueProperty, label, true);
            }

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            position.x += position.width + 24.0f;
            position.width = position.height = EditorGUI.GetPropertyHeight(enabledProperty);
            position.x -= position.width;
            
            EditorGUI.PropertyField(position, enabledProperty, GUIContent.none);
            
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}