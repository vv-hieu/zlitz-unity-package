using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Zlitz.Utility.Serializables.Runtime
{
    public class FunctionReferenceHelper
    {
        public static IEnumerable<string> GetMethodNames(Type targetType, Type returnType, params Type[] paramTypes)
        {
            if (targetType == null)
            {
                return null;
            }

            MethodInfo[] result = targetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            result = result.Where(m => MatchSignature(m, returnType, paramTypes)).ToArray();

            return result == null ? null : result.Select(m => m.Name);
        }

        private static bool MatchSignature(MethodInfo method, Type returnType, Type[] paramTypes)
        {
            if (method == null || method.ReturnType != returnType)
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
