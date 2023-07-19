using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zlitz.Utility.Serializables.Runtime
{
    [Serializable]
    public class FunctionReference<TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type                       m_currentTargetType;
        private InvokableFunction<TReturn> m_function;

        public Type targetType
        {
            get => m_targetType == null ? null : m_targetType;
            set => m_targetType = value;
        }

        public string methodName
        {
            get => m_methodName;
            set => m_methodName = value;
        }

        public TReturn Invoke(GameObject target)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                if (m_function != null)
                {
                    return m_function.Invoke(component);
                }
            }
            throw new NullReferenceException("Unable to invoke function");
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_function = new InvokableFunction<TReturn>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn)));
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            bool typeChanged = false;
            if (m_currentTargetType != null && m_targetType == null)
            {
                typeChanged = true;
            }
            else if (m_currentTargetType != null && m_targetType != null)
            {
                typeChanged = m_currentTargetType != m_targetType.type;
            }

            if (typeChanged)
            {
                m_currentTargetType = m_targetType;
                m_function = new InvokableFunction<TReturn>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn)));
                }
            }
        }
    }

    [Serializable]
    public class FunctionReference<T0, TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type                           m_currentTargetType;
        private InvokableFunction<T0, TReturn> m_function;

        public Type targetType
        {
            get => m_targetType == null ? null : m_targetType;
            set => m_targetType = value;
        }

        public string methodName
        {
            get => m_methodName;
            set => m_methodName = value;
        }

        public TReturn Invoke(GameObject target, T0 arg0)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                if (m_function != null)
                {
                    return m_function.Invoke(component, arg0);
                }
            }
            throw new NullReferenceException("Unable to invoke function");
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_function = new InvokableFunction<T0, TReturn>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn), typeof(T0)));
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            bool typeChanged = false;
            if (m_currentTargetType != null && m_targetType == null)
            {
                typeChanged = true;
            }
            else if (m_currentTargetType != null && m_targetType != null)
            {
                typeChanged = m_currentTargetType != m_targetType.type;
            }

            if (typeChanged)
            {
                m_currentTargetType = m_targetType;
                m_function = new InvokableFunction<T0, TReturn>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn), typeof(T0)));
                }
            }
        }
    }

    [Serializable]
    public class FunctionReference<T0, T1, TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type                               m_currentTargetType;
        private InvokableFunction<T0, T1, TReturn> m_function;

        public Type targetType
        {
            get => m_targetType == null ? null : m_targetType;
            set => m_targetType = value;
        }

        public string methodName
        {
            get => m_methodName;
            set => m_methodName = value;
        }

        public TReturn Invoke(GameObject target, T0 arg0, T1 arg1)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                if (m_function != null)
                {
                    return m_function.Invoke(component, arg0, arg1);
                }
            }
            throw new NullReferenceException("Unable to invoke function");
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_function = new InvokableFunction<T0, T1, TReturn>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn), typeof(T0), typeof(T1)));
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            bool typeChanged = false;
            if (m_currentTargetType != null && m_targetType == null)
            {
                typeChanged = true;
            }
            else if (m_currentTargetType != null && m_targetType != null)
            {
                typeChanged = m_currentTargetType != m_targetType.type;
            }

            if (typeChanged)
            {
                m_currentTargetType = m_targetType;
                m_function = new InvokableFunction<T0, T1, TReturn>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn), typeof(T0), typeof(T1)));
                }
            }
        }
    }

    [Serializable]
    public class FunctionReference<T0, T1, T2, TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type m_currentTargetType;
        private InvokableFunction<T0, T1, T2, TReturn> m_function;

        public Type targetType
        {
            get => m_targetType == null ? null : m_targetType;
            set => m_targetType = value;
        }

        public string methodName
        {
            get => m_methodName;
            set => m_methodName = value;
        }

        public TReturn Invoke(GameObject target, T0 arg0, T1 arg1, T2 arg2)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                if (m_function != null)
                {
                    return m_function.Invoke(component, arg0, arg1, arg2);
                }
            }
            throw new NullReferenceException("Unable to invoke function");
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_function = new InvokableFunction<T0, T1, T2, TReturn>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn), typeof(T0), typeof(T1), typeof(T2)));
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            bool typeChanged = false;
            if (m_currentTargetType != null && m_targetType == null)
            {
                typeChanged = true;
            }
            else if (m_currentTargetType != null && m_targetType != null)
            {
                typeChanged = m_currentTargetType != m_targetType.type;
            }

            if (typeChanged)
            {
                m_currentTargetType = m_targetType;
                m_function = new InvokableFunction<T0, T1, T2, TReturn>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn), typeof(T0), typeof(T1), typeof(T2)));
                }
            }
        }
    }

    [Serializable]
    public class FunctionReference<T0, T1, T2, T3, TReturn> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type                                       m_currentTargetType;
        private InvokableFunction<T0, T1, T2, T3, TReturn> m_function;

        public Type targetType
        {
            get => m_targetType == null ? null : m_targetType;
            set => m_targetType = value;
        }

        public string methodName
        {
            get => m_methodName;
            set => m_methodName = value;
        }

        public TReturn Invoke(GameObject target, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                if (m_function != null)
                {
                    return m_function.Invoke(component, arg0, arg1, arg2, arg3);
                }
            }
            throw new NullReferenceException("Unable to invoke function");
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_function = new InvokableFunction<T0, T1, T2, T3, TReturn>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn), typeof(T0), typeof(T1), typeof(T2), typeof(T3)));
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            bool typeChanged = false;
            if (m_currentTargetType != null && m_targetType == null)
            {
                typeChanged = true;
            }
            else if (m_currentTargetType != null && m_targetType != null)
            {
                typeChanged = m_currentTargetType != m_targetType.type;
            }

            if (typeChanged)
            {
                m_currentTargetType = m_targetType;
                m_function = new InvokableFunction<T0, T1, T2, T3, TReturn>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(TReturn), typeof(T0), typeof(T1), typeof(T2), typeof(T3)));
                }
            }
        }
    }
}
