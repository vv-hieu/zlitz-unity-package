using System;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BehaviourTreeNodeDecoratorAttribute : Attribute
    {
        private string m_name;

        public string name => m_name;

        public BehaviourTreeNodeDecoratorAttribute(string name)
        {
            m_name = name;
        }
    }
}
