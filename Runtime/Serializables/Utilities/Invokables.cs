using System;
using System.Linq;
using System.Reflection;

namespace Zlitz.Utility.Serializables.Runtime
{
    public class InvokableAction : IAction
    {
        private MethodInfo m_method;

        public void Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args);
            m_method?.Invoke(target, args);
        }
    
        public void Invoke(object target)
        {
            m_method?.Invoke(target, new object[] { });
        }

        public InvokableAction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(void));
        }
    }

    public class InvokableAction<T0> : IAction
    {
        private MethodInfo m_method;

        public void Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args, typeof(T0));
            m_method?.Invoke(target, args);
        }

        public void Invoke(object target, T0 arg0)
        {
            m_method?.Invoke(target, new object[] { arg0 });
        }

        public InvokableAction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(void), typeof(T0));
        }
    }

    public class InvokableAction<T0, T1> : IAction
    {
        private MethodInfo m_method;

        public void Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args, typeof(T0), typeof(T1));
            m_method?.Invoke(target, args);
        }

        public void Invoke(object target, T0 arg0, T1 arg1)
        {
            m_method?.Invoke(target, new object[] { arg0, arg1 });
        }

        public InvokableAction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(void), typeof(T0), typeof(T1));
        }
    }

    public class InvokableAction<T0, T1, T2> : IAction
    {
        private MethodInfo m_method;

        public void Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args, typeof(T0), typeof(T1), typeof(T2));
            m_method?.Invoke(target, args);
        }

        public void Invoke(object target, T0 arg0, T1 arg1, T2 arg2)
        {
            m_method?.Invoke(target, new object[] { arg0, arg1, arg2 });
        }

        public InvokableAction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(void), typeof(T0), typeof(T1), typeof(T2));
        }
    }

    public class InvokableAction<T0, T1, T2, T3> : IAction
    {
        private MethodInfo m_method;

        public void Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args, typeof(T0), typeof(T1), typeof(T2), typeof(T3));
            m_method?.Invoke(target, args);
        }

        public void Invoke(object target, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            m_method?.Invoke(target, new object[] { arg0, arg1, arg2, arg3 });
        }

        public InvokableAction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(void), typeof(T0), typeof(T1), typeof(T2), typeof(T3));
        }
    }

    public interface IAction
    {
        void Invoke(object target, params object[] args);
    }

    public class InvokableFunction<TResult> : IFunction<TResult>
    {
        private MethodInfo m_method;

        public TResult Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args);
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, args);
        }

        public TResult Invoke(object target)
        {
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, new object[] { });
        }

        public InvokableFunction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(TResult));
        }
    }

    public class InvokableFunction<T0, TResult> : IFunction<TResult>
    {
        private MethodInfo m_method;

        public TResult Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args, typeof(T0));
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, args);
        }

        public TResult Invoke(object target, T0 arg0)
        {
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, new object[] { arg0 });
        }

        public InvokableFunction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(TResult), typeof(T0));
        }
    }

    public class InvokableFunction<T0, T1, TResult> : IFunction<TResult>
    {
        private MethodInfo m_method;

        public TResult Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args, typeof(T0), typeof(T1));
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, args);
        }

        public TResult Invoke(object target, T0 arg0, T1 arg1)
        {
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, new object[] { arg0, arg1 });
        }

        public InvokableFunction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(TResult), typeof(T0), typeof(T1));
        }
    }

    public class InvokableFunction<T0, T1, T2, TResult> : IFunction<TResult>
    {
        private MethodInfo m_method;

        public TResult Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args, typeof(T0), typeof(T1), typeof(T2));
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, args);
        }

        public TResult Invoke(object target, T0 arg0, T1 arg1, T2 arg2)
        {
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, new object[] { arg0, arg1, arg2 });
        }

        public InvokableFunction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(TResult), typeof(T0), typeof(T1), typeof(T2));
        }
    }

    public class InvokableFunction<T0, T1, T2, T3, TResult> : IFunction<TResult>
    {
        private MethodInfo m_method;

        public TResult Invoke(object target, params object[] args)
        {
            InvokableHelper.AssertArgumentSignature(args, typeof(T0), typeof(T1), typeof(T2), typeof(T3));
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, args);
        }

        public TResult Invoke(object target, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (m_method == null)
            {
                return default;
            }
            return (TResult)m_method.Invoke(target, new object[] { arg0, arg1, arg2, arg3 });
        }

        public InvokableFunction(Type targetType, string methodName)
        {
            m_method = InvokableHelper.GetMethod(targetType, methodName, typeof(TResult), typeof(T0), typeof(T1), typeof(T2), typeof(T3));
        }
    }

    public interface IFunction<TResult>
    {
        TResult Invoke(object target, params object[] args);
    }

    internal static class InvokableHelper 
    { 
        public static bool CheckArguments(object[] args, Type[] expectedTypes)
        {
            int nArgs          = args == null ? 0 : args.Length;
            int nExpectedTypes = expectedTypes == null ? 0 : expectedTypes.Length;

            if (nArgs != nExpectedTypes)
            {
                return false;
            }

            if (nArgs == 0)
            {
                return true;
            }

            for (int i = 0; i < nArgs; i++)
            {
                if (expectedTypes[i] == null || !expectedTypes[i].IsInstanceOfType(args[i]))
                {
                    return false;
                }
            }

            return true;
        }
    
        public static void AssertArgumentSignature(object[] args, params Type[] expectedTypes)
        {
            if (!CheckArguments(args, expectedTypes))
            {
                throw new ArgumentException("Invalid arguments");
            }
        }
 
        public static MethodInfo GetMethod(Type targetType, string methodName, Type returnType, params Type[] paramTypes)
        {
            if (targetType == null)
            {
                return null;
            }

            MethodInfo[] result = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            result = result.Where(m => MatchMethodDescription(m, methodName, returnType, paramTypes)).ToArray();
            if (result == null || result.Length <= 0)
            {
                return null;
            }

            return result.First();
        }

        private static bool MatchMethodDescription(MethodInfo method, string methodName, Type returnType, Type[] paramTypes)
        {
            if (method == null || method.Name != methodName || method.ReturnType != returnType)
            {
                return false;
            }

            ParameterInfo[] parameters = method.GetParameters();

            int l1 = parameters == null ? 0 : parameters.Length;
            int l2 = paramTypes == null ? 0 : paramTypes.Length;

            if (l1 != l2)
            {
                return false;
            }
            
            if (l1 > 0)
            {
                for (int i = 0; i < l1; i++)
                {
                    if (parameters[i].ParameterType != paramTypes[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
