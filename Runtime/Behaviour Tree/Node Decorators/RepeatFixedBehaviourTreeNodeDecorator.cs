using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNodeDecorator("Repeat (Fixed)")]
    public class RepeatFixedBehaviourTreeNodeDecorator : BehaviourTreeNodeDecorator
    {
        [SerializeField, Min(0)]
        private int m_iterations = 1;

        public override BehaviourTreeNodeDecoratorState CreateDecoratorState()
        {
            return new RepeatFixedBehaviourTreeNodeDecoratorState();
        }

        public override void OnStart(BehaviourTreeState state)
        {
            RepeatFixedBehaviourTreeNodeDecoratorState decoratorState = state.GetDecoratorState(this) as RepeatFixedBehaviourTreeNodeDecoratorState;
            decoratorState.currentIteration = 0;
        }

        public override BehaviourResult Execute(BehaviourTreeState state, BehaviourResult nodeResult)
        {
            if (nodeResult == BehaviourResult.Failed)
            {
                return BehaviourResult.Failed;
            }
            if (nodeResult == BehaviourResult.Successful)
            {
                RepeatFixedBehaviourTreeNodeDecoratorState decoratorState = state.GetDecoratorState(this) as RepeatFixedBehaviourTreeNodeDecoratorState;
                decoratorState.currentIteration++;
                if (decoratorState.currentIteration >= m_iterations)
                {
                    return BehaviourResult.Successful;
                }
            }
            return BehaviourResult.Running;
        }

        public override string GetDecoratorDescription()
        {
            return $"Repeat {m_iterations} time(s)";
        }
    }

    internal class RepeatFixedBehaviourTreeNodeDecoratorState : BehaviourTreeNodeDecoratorState
    {
        private int m_currentIteration;

        public int currentIteration
        {
            get => m_currentIteration;
            set => m_currentIteration = value;
        }

        public RepeatFixedBehaviourTreeNodeDecoratorState()
        {
            m_currentIteration = 0;
        }
    }
}
