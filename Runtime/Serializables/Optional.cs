using System;
using UnityEngine;

namespace Zlitz.Utility.Serializables.Runtime
{
    [Serializable]
    public struct Optional<T> where T : struct
    {
        [SerializeField] private bool m_enabled;
        [SerializeField] private T    m_value;

        public bool enabled => m_enabled;

        public T value => m_value;

        public static implicit operator T? (Optional<T> optional) => optional.m_enabled? optional.m_value : null;
        
        public static explicit operator Optional<T>(T? value) => value.HasValue ? Of(value.Value) : Empty();

        public static Optional<T> Empty()
        {
            Optional<T> res = new Optional<T>();

            res.m_enabled = false;
            res.m_value = default(T);

            return res;
        }

        public static Optional<T> Of(T value)
        {
            Optional<T> res = new Optional<T>();

            res.m_enabled = true;
            res.m_value = value;

            return res;
        }
    }
}
