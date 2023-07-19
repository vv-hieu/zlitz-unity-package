using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNodeDecorator("Force Result")]
    public class ForceResultBehaviourTreeNodeDecorator : BehaviourTreeNodeDecorator
    {
        [SerializeField]
        private bool m_successful = true;

        public override BehaviourResult Execute(BehaviourTreeState state, BehaviourResult nodeResult)
        {
            if (nodeResult != BehaviourResult.Running)
            {
                return m_successful ? BehaviourResult.Successful : BehaviourResult.Failed;
            }
            return BehaviourResult.Running;
        }

        public override string GetDecoratorDescription()
        {
            string result = m_successful ? "success" : "failure";
            return $"Force {result}";
        }
    }
}
