using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace Zlitz.Utility.Serializables.Runtime
{
    [Serializable]
    public class ActionReference : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type            m_currentTargetType;
        private InvokableAction m_action;

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

        public void Invoke(GameObject target)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                m_action?.Invoke(component);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_action = new InvokableAction(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void)));
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
                m_action = new InvokableAction(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void)));
                }
            }
        }
    }

    [Serializable]
    public class ActionReference<T0> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type                m_currentTargetType;
        private InvokableAction<T0> m_action;

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

        public void Invoke(GameObject target, T0 arg0)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                m_action?.Invoke(component, arg0);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_action = new InvokableAction<T0>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void), typeof(T0)));
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
                m_action = new InvokableAction<T0>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void), typeof(T0)));
                }
            }
        }
    }

    [Serializable]
    public class ActionReference<T0, T1> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type                    m_currentTargetType;
        private InvokableAction<T0, T1> m_action;

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

        public void Invoke(GameObject target, T0 arg0, T1 arg1)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                m_action?.Invoke(component, arg0, arg1);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_action = new InvokableAction<T0, T1>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void), typeof(T0), typeof(T1)));
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
                m_action = new InvokableAction<T0, T1>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void), typeof(T0), typeof(T1)));
                }
            }
        }
    }

    [Serializable]
    public class ActionReference<T0, T1, T2> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type                        m_currentTargetType;
        private InvokableAction<T0, T1, T2> m_action;

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

        public void Invoke(GameObject target, T0 arg0, T1 arg1, T2 arg2)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                m_action?.Invoke(component, arg0, arg1, arg2);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_action = new InvokableAction<T0, T1, T2>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void), typeof(T0), typeof(T1), typeof(T2)));
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
                m_action = new InvokableAction<T0, T1, T2>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void), typeof(T0), typeof(T1), typeof(T2)));
                }
            }
        }
    }

    [Serializable]
    public class ActionReference<T0, T1, T2, T3> : ISerializationCallbackReceiver
    {
        [SerializeField, TypeReferenceOptions(inherits: typeof(Component))]
        private TypeReference m_targetType;

        [SerializeField]
        private string m_methodName;

        [SerializeField]
        private List<string> m_availableMethodNames;

        private Type                            m_currentTargetType;
        private InvokableAction<T0, T1, T2, T3> m_action;

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

        public void Invoke(GameObject target, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (m_targetType.type != null && target.TryGetComponent(m_targetType, out Component component))
            {
                m_action?.Invoke(component, arg0, arg1, arg2, arg3);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_currentTargetType = m_targetType;
            m_action = new InvokableAction<T0, T1, T2, T3>(m_targetType, m_methodName);

            m_availableMethodNames = new List<string>();
            if (m_currentTargetType != null)
            {
                m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void), typeof(T0), typeof(T1), typeof(T2), typeof(T3)));
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
                m_action = new InvokableAction<T0, T1, T2, T3>(m_targetType, m_methodName);

                m_availableMethodNames = new List<string>();
                if (m_currentTargetType != null)
                {
                    m_availableMethodNames.AddRange(FunctionReferenceHelper.GetMethodNames(m_currentTargetType, typeof(void), typeof(T0), typeof(T1), typeof(T2), typeof(T3)));
                }
            }
        }
    }
}
