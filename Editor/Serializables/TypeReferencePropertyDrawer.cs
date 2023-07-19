using System;
using System.Linq;
using System.Collections.Specialized;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using Zlitz.Utility.Serializables.Runtime;

namespace Zlitz.Utility.Serializables.Editor
{
    [CustomPropertyDrawer(typeof(TypeReference))]
    [CustomPropertyDrawer(typeof(TypeReferenceOptionsAttribute), true)]
    public class TypeReferencePropertyDrawer : PropertyDrawer
    {
        private static Dictionary<UnityEngine.Object, (TypeReferenceDropdown, AdvancedDropdownState)> s_dropdowns;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0.0f);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (label != null && label != GUIContent.none)
            {
                position = EditorGUI.PrefixLabel(position, label);
            }

            TypeReferenceOptionsAttribute typeReferenceOptionsAttribute = attribute as TypeReferenceOptionsAttribute ?? new TypeReferenceOptionsAttribute();

            SerializedProperty typeNameAndAssemblyProperty = property.FindPropertyRelative("m_typeNameAndAssembly");
            
            Type currentType = Type.GetType(typeNameAndAssemblyProperty.stringValue);

            if (currentType == null || !CheckInherits(currentType, typeReferenceOptionsAttribute.inherits))
            {
                typeNameAndAssemblyProperty.stringValue = "";
                typeNameAndAssemblyProperty.serializedObject.ApplyModifiedProperties();
            }

            string typeName = typeNameAndAssemblyProperty.stringValue;
            int splitIndex = typeName.IndexOf(",");
            if (splitIndex >= 0)
            {
                typeName = typeName.Substring(0, splitIndex);
            }
            if (typeName == "")
            {
                typeName = "(None)";
            }


            if (GUI.Button(position, new GUIContent(typeName), EditorStyles.popup))
            {
                AdvancedDropdownState state = new AdvancedDropdownState();
                TypeReferenceDropdown dropdown = new TypeReferenceDropdown(state, typeReferenceOptionsAttribute, (type) =>
                {
                    string typeNameAndAssembly = "";
                    if (type != null && type.FullName != null)
                    {
                        string assemblyFullName = type.Assembly.FullName;
                        int commaIndex = assemblyFullName.IndexOf(',');

                        typeNameAndAssembly = $"{type.FullName},{assemblyFullName.Substring(0, commaIndex)}";
                    }

                    typeNameAndAssemblyProperty.stringValue = typeNameAndAssembly;
                    typeNameAndAssemblyProperty.serializedObject.ApplyModifiedProperties();
                });
                dropdown.Show(position);
            }
        }

        private class TypeReferenceDropdown : AdvancedDropdown
        {
            private Type m_inherits;

            private TypeReferenceOptionsAttribute.Grouping m_grouping;

            private TypeSelectCallback m_onTypeSelect;

            private static readonly HashSet<string> s_systemAssemblyNames = new HashSet<string> { "mscorlib", "System", "System.Core" };

            public TypeReferenceDropdown(AdvancedDropdownState state, TypeReferenceOptionsAttribute option, TypeSelectCallback onTypeSelect) : base(state)
            {
                m_inherits = option.inherits;
                m_grouping = option.grouping;

                m_onTypeSelect = onTypeSelect;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                AdvancedDropdownItem root = new AdvancedDropdownItem("Select type");

                IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().Where(a => !s_systemAssemblyNames.Contains(a.GetName().Name)).SelectMany(a => a.GetTypes().Where(t => t.IsVisible && !t.IsAbstract && CheckInherits(t, m_inherits)));
                if (types != null)
                {
                    root.AddChild(new TypeReferenceDropdownItem("(None)", null));
                    if (m_grouping == TypeReferenceOptionsAttribute.Grouping.None)
                    {
                        SortedDictionary<string, AdvancedDropdownItem> items = new SortedDictionary<string, AdvancedDropdownItem>();

                        foreach (Type type in types)
                        {
                            if (TryGetTypeNameAndAssembly(type, out string typeName))
                            {
                                items.TryAdd(typeName, new TypeReferenceDropdownItem(typeName, type));
                            }
                        }

                        foreach (AdvancedDropdownItem item in items.Values)
                        {
                            root.AddChild(item);
                        }
                    }
                    else if (m_grouping == TypeReferenceOptionsAttribute.Grouping.Namespace)
                    {
                        SortedDictionary<string, AdvancedDropdownItem> namespaceItems = new SortedDictionary<string, AdvancedDropdownItem>();

                        foreach (Type type in types)
                        {
                            if (TryGetTypeNameAndAssembly(type, out string typeName))
                            {
                                string typeNamespace = "(Unknown)";

                                int splitIndex = typeName.LastIndexOf('.');
                                if (splitIndex >= 0)
                                {
                                    typeNamespace = typeName.Substring(0, splitIndex);
                                    typeName      = typeName.Substring(splitIndex + 1);
                                }

                                if (!namespaceItems.TryGetValue(typeNamespace, out AdvancedDropdownItem namespaceItem))
                                {
                                    namespaceItem = new AdvancedDropdownItem(typeNamespace);
                                    namespaceItems.Add(typeNamespace, namespaceItem);
                                }

                                namespaceItem.AddChild(new TypeReferenceDropdownItem(typeName, type));
                            }
                        }

                        foreach (AdvancedDropdownItem item in namespaceItems.Values)
                        {
                            root.AddChild(item);
                        }
                    }
                }

                return root;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                if (item is TypeReferenceDropdownItem typeItem)
                {
                    m_onTypeSelect?.Invoke(typeItem.type);
                }
            }

            private static bool TryGetTypeNameAndAssembly(Type type, out string typeName)
            {
                typeName = "";

                if (type != null && type.FullName != null)
                {
                    typeName = type.FullName;
                    return true;
                }

                return false;
            }

            private static void GetTypes(UnityEngine.Object target)
            {

            }

            private class TypeReferenceDropdownItem : AdvancedDropdownItem
            {
                private Type m_type;

                public Type type => m_type;

                public TypeReferenceDropdownItem(string name, Type type) : base(name)
                {
                    m_type = type;
                }
            }

            public delegate void TypeSelectCallback(Type type);
        }

        private static bool CheckInherits(Type type, Type inherits)
        {
            if (inherits == null)
            {
                return true;
            }
            return type.IsSubclassOf(inherits);
        }

    }
}