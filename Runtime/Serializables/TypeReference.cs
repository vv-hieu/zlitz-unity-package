using System;
using UnityEngine;

namespace Zlitz.Utility.Serializables.Runtime
{
    [Serializable]
    public class TypeReference : ISerializationCallbackReceiver
    {
        [SerializeField]
        private string m_typeNameAndAssembly;

        private Type m_type;

        public Type type
        {
            get
            {
                if (m_type == null)
                {
                    m_type = GetType(m_typeNameAndAssembly);
                }
                return m_type;
            }
            set
            {
                if (TryGetTypeNameAndAssembly(value, out string typeNameAndAssembly))
                {
                    m_type = value;
                    m_typeNameAndAssembly = typeNameAndAssembly;
                }
                else
                {
                    m_type = null;
                    m_typeNameAndAssembly = "";
                }
            }
        }

        public static implicit operator Type(TypeReference typeReference) => typeReference?.type;

        public static implicit operator TypeReference(Type type) => new TypeReference(type);

        public TypeReference(Type type = null)
        {
            if (TryGetTypeNameAndAssembly(type, out string typeNameAndAssembly))
            {
                m_type = type;
                m_typeNameAndAssembly = typeNameAndAssembly;
            }
            else
            {
                m_type = null;
                m_typeNameAndAssembly = "";
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_type = GetType(m_typeNameAndAssembly);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (TryGetTypeNameAndAssembly(m_type, out string typeNameAndAssembly))
            {
                m_typeNameAndAssembly = typeNameAndAssembly;
            }
            else
            {
                m_typeNameAndAssembly = "";
            }
        }

        private static void SplitTypeNameAndAssembly(string typeNameAndAssembly, out string typeName, out string assembly)
        {
            typeName = typeNameAndAssembly;
            assembly = "";
            int splitIndex = typeName.IndexOf(',');
            if (splitIndex >= 0)
            {
                assembly = typeName.Substring(splitIndex + 1);
                typeName = typeName.Substring(0, splitIndex);
            }
        }

        private static Type GetType(string typeNameAndAssembly)
        {
            return Type.GetType(typeNameAndAssembly);
        }

        private static bool TryGetTypeNameAndAssembly(Type type, out string typeNameAndAssembly)
        {
            typeNameAndAssembly = "";

            if (type != null && type.FullName != null)
            {
                string assemblyFullName = type.Assembly.FullName;
                int commaIndex = assemblyFullName.IndexOf(',');

                typeNameAndAssembly = $"{type.FullName},{assemblyFullName.Substring(0, commaIndex)}";

                return true;
            }

            return false;
        }
    }
}
