using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using UnityEditor.Callbacks;

using Zlitz.AI.BehaviourTrees.Runtime;

namespace Zlitz.AI.BehaviourTrees.Editor
{
    public static class BehaviourTreeNodes
    {
        private static IDictionary<Type, string> s_behavoiurTreeNodeTypes;

        public static IDictionary<Type, string> GetBehaviourTreeNodeTypes()
        {
            if (s_behavoiurTreeNodeTypes != null)
            {
                return s_behavoiurTreeNodeTypes;
            }

            s_behavoiurTreeNodeTypes = new Dictionary<Type, string>();

            IEnumerable<Type>   nodeTypes     = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(BehaviourTreeNodeAttribute)) && !t.IsAbstract));
            IEnumerable<string> nodeTypeNames = nodeTypes.Select(t => GetBehaviourTreeNodeTypeName(t));

            IEnumerator<Type>   nodeType     = nodeTypes.GetEnumerator();
            IEnumerator<string> nodeTypeName = nodeTypeNames.GetEnumerator();

            while (nodeType.MoveNext() && nodeTypeName.MoveNext())
            {
                s_behavoiurTreeNodeTypes.Add(nodeType.Current, nodeTypeName.Current);
            }

            return s_behavoiurTreeNodeTypes;
        }

        public static string GetBehaviourTreeNodeTypeName(Type type)
        {
            if (s_behavoiurTreeNodeTypes != null && s_behavoiurTreeNodeTypes.TryGetValue(type, out string result))
            {
                return result;
            }

            BehaviourTreeNodeAttribute attribute = type.GetCustomAttribute<BehaviourTreeNodeAttribute>();
            if (attribute != null)
            {
                return attribute.name;
            }

            return "";
        }

        public static Type GetBehaviourTreeNodeType(string name)
        {
            if (s_behavoiurTreeNodeTypes != null)
            {
                foreach (var p in s_behavoiurTreeNodeTypes)
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
            s_behavoiurTreeNodeTypes = null;
            GetBehaviourTreeNodeTypes();
        }
    }
}
