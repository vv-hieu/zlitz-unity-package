using System;
using UnityEngine;

namespace Zlitz.Utility.Serializables.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeReferenceOptionsAttribute : PropertyAttribute
    {
        private Type m_inherits;

        private Grouping m_grouping;

        public Type inherits => m_inherits;

        public Grouping grouping => m_grouping;

        public TypeReferenceOptionsAttribute(Type inherits = null, Grouping grouping = Grouping.Namespace)
        {
            m_inherits = inherits;
            m_grouping = grouping;
        }

        public enum Grouping
        {
            None,
            Namespace
        }
    }
}