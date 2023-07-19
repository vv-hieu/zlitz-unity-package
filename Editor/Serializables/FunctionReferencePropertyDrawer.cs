using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Zlitz.Utility.Serializables.Runtime;

namespace Zlitz.Utility.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(FunctionReference<>))]
    [CustomPropertyDrawer(typeof(FunctionReference<,>))]
    [CustomPropertyDrawer(typeof(FunctionReference<,,>))]
    [CustomPropertyDrawer(typeof(FunctionReference<,,,>))]
    [CustomPropertyDrawer(typeof(FunctionReference<,,,,>))]
    public class FunctionReferencePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 2.0f * EditorStyles.popup.CalcHeight(GUIContent.none, 0.0f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.serializedObject.Update();

            position.height = EditorStyles.popup.CalcHeight(GUIContent.none, 0.0f);
            Rect position2 = position;
            if (label != null && label != GUIContent.none)
            {
                position2 = EditorGUI.PrefixLabel(position, label);
            }

            SerializedFunctionReference actionReference = new SerializedFunctionReference(property);

            EditorGUI.PropertyField(position2, actionReference.targetTypeProperty, GUIContent.none);
            property.serializedObject.ApplyModifiedProperties();

            position.y += position.height;

            int indent = EditorGUI.indentLevel;

            EditorGUI.indentLevel++;
            position = EditorGUI.PrefixLabel(position, new GUIContent("Method"));
            EditorGUI.indentLevel--;

            int currentIndex = 0;
            List<GUIContent> availableMethodNames = new List<GUIContent>();
            availableMethodNames.Add(new GUIContent("(None)"));
            for (int i = 0; i < actionReference.availableMethodNamesProperty.arraySize; i++)
            {
                string methodName = actionReference.availableMethodNamesProperty.GetArrayElementAtIndex(i).stringValue;
                availableMethodNames.Add(new GUIContent(methodName));

                if (actionReference.methodNameProperty.stringValue == methodName)
                {
                    currentIndex = i + 1;
                }
            }

            EditorGUI.indentLevel = 0;

            int selectedIndex = EditorGUI.Popup(position, currentIndex, availableMethodNames.ToArray());
            if (selectedIndex != currentIndex)
            {
                actionReference.methodNameProperty.stringValue = availableMethodNames[selectedIndex].text;
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel = indent;
        }

        private class SerializedFunctionReference
        {
            private SerializedProperty m_targetTypeProperty;
            private SerializedProperty m_methodNameProperty;
            private SerializedProperty m_availableMethodNamesProperty;

            public SerializedProperty targetTypeProperty => m_targetTypeProperty;

            public SerializedProperty methodNameProperty => m_methodNameProperty;

            public SerializedProperty availableMethodNamesProperty => m_availableMethodNamesProperty;

            public SerializedFunctionReference(SerializedProperty property)
            {
                m_targetTypeProperty           = property.FindPropertyRelative("m_targetType");
                m_methodNameProperty           = property.FindPropertyRelative("m_methodName");
                m_availableMethodNamesProperty = property.FindPropertyRelative("m_availableMethodNames");
            }
        }
    }
}
