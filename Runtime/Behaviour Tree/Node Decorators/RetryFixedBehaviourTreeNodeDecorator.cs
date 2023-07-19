using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNodeDecorator("Retry (Fixed)")]
    public class RetryFixedBehaviourTreeNodeDecorator : BehaviourTreeNodeDecorator
    {
        [SerializeField, Min(0)]
        private int m_iterations = 1;

        public override BehaviourTreeNodeDecoratorState CreateDecoratorState()
        {
            return new RetryFixedBehaviourTreeNodeDecoratorState();
        }

        public override void OnStart(BehaviourTreeState state)
        {
            RetryFixedBehaviourTreeNodeDecoratorState decoratorState = state.GetDecoratorState(this) as RetryFixedBehaviourTreeNodeDecoratorState;
            decoratorState.currentIteration = 0;
        }

        public override BehaviourResult Execute(BehaviourTreeState state, BehaviourResult nodeResult)
        {
            if (nodeResult == BehaviourResult.Successful)
            {
                return BehaviourResult.Successful;
            }
            if (nodeResult == BehaviourResult.Failed)
            {
                RetryFixedBehaviourTreeNodeDecoratorState decoratorState = state.GetDecoratorState(this) as RetryFixedBehaviourTreeNodeDecoratorState;
                decoratorState.currentIteration++;
                if (decoratorState.currentIteration >= m_iterations)
                {
                    return BehaviourResult.Failed;
                }
            }
            return BehaviourResult.Running;
        }

        public override string GetDecoratorDescription()
        {
            return $"Retry {m_iterations} time(s)";
        }
    }

    internal class RetryFixedBehaviourTreeNodeDecoratorState : BehaviourTreeNodeDecoratorState
    {
        private int m_currentIteration;

        public int currentIteration
        {
            get => m_currentIteration;
            set => m_currentIteration = value;
        }

        public RetryFixedBehaviourTreeNodeDecoratorState()
        {
            m_currentIteration = 0;
        }
    }
}
