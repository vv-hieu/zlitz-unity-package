using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor.Callbacks;

using Zlitz.AI.BehaviourTrees.Runtime;

namespace Zlitz.AI.BehaviourTrees.Editor
{
    public static class BehaviourTreeNodeDecorators
    {
        private static IDictionary<Type, string> s_behavoiurTreeNodeDecoratorTypes;

        public static IDictionary<Type, string> GetBehaviourTreeNodeDecoratorTypes()
        {
            if (s_behavoiurTreeNodeDecoratorTypes != null)
            {
                return s_behavoiurTreeNodeDecoratorTypes;
            }

            s_behavoiurTreeNodeDecoratorTypes = new Dictionary<Type, string>();

            IEnumerable<Type>   decoratorTypes     = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(BehaviourTreeNodeDecoratorAttribute)) && !t.IsAbstract));
            IEnumerable<string> decoratorTypeNames = decoratorTypes.Select(t => GetBehaviourTreeNodeDecoratorTypeName(t));

            IEnumerator<Type>   decoratorType     = decoratorTypes.GetEnumerator();
            IEnumerator<string> decoratorTypeName = decoratorTypeNames.GetEnumerator();

            while (decoratorType.MoveNext() && decoratorTypeName.MoveNext())
            {
                s_behavoiurTreeNodeDecoratorTypes.Add(decoratorType.Current, decoratorTypeName.Current);
            }

            return s_behavoiurTreeNodeDecoratorTypes;
        }

        public static string GetBehaviourTreeNodeDecoratorTypeName(Type type)
        {
            if (s_behavoiurTreeNodeDecoratorTypes != null && s_behavoiurTreeNodeDecoratorTypes.TryGetValue(type, out string result))
            {
                return result;
            }

            BehaviourTreeNodeDecoratorAttribute attribute = type.GetCustomAttribute<BehaviourTreeNodeDecoratorAttribute>();
            if (attribute != null)
            {
                return attribute.name;
            }

            return "";
        }

        public static Type GetBehaviourTreeNodeDecoratorType(string name)
        {
            if (s_behavoiurTreeNodeDecoratorTypes != null)
            {
                foreach (var p in s_behavoiurTreeNodeDecoratorTypes)
                {
                    if (p.Value == name)
                    {
                        return p.Key;
                    }
                }
            }

            return null;
        }

        [DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            s_behavoiurTreeNodeDecoratorTypes = null;
            GetBehaviourTreeNodeDecoratorTypes();
        }
    }
}
