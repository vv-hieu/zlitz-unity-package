using System;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BehaviourTreeNodeAttribute : Attribute
    {
        private string m_name;

        public string name => m_name;

        public BehaviourTreeNodeAttribute(string name)
        {
            m_name = name;
        }
    }
}
