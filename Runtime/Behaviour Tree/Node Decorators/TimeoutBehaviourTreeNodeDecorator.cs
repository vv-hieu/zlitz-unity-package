using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNodeDecorator("Timeout")]
    public class TimeoutBehaviourTreeNodeDecorator : BehaviourTreeNodeDecorator
    {
        [SerializeField, Min(0.0f)]
        private float m_time;

        public override BehaviourTreeNodeDecoratorState CreateDecoratorState()
        {
            return new TimeoutBehaviourTreeNodeDecoratorState();
        }

        public override void OnStart(BehaviourTreeState state)
        {
            base.OnStart(state);

            TimeoutBehaviourTreeNodeDecoratorState decoratorState = state.GetDecoratorState(this) as TimeoutBehaviourTreeNodeDecoratorState;
            decoratorState.time = 0.0f;
        }

        public override BehaviourResult Execute(BehaviourTreeState state, BehaviourResult nodeResult)
        {
            TimeoutBehaviourTreeNodeDecoratorState decoratorState = state.GetDecoratorState(this) as TimeoutBehaviourTreeNodeDecoratorState;
            decoratorState.time += Time.deltaTime;

            if (nodeResult != BehaviourResult.Running)
            {
                return nodeResult;
            }

            if (decoratorState.time >= m_time)
            {
                return BehaviourResult.Failed;
            }

            return BehaviourResult.Running;
        }

        public override string GetDecoratorDescription()
        {
            return $"Timeout {m_time} second(s)";
        }
    }

    internal class TimeoutBehaviourTreeNodeDecoratorState : BehaviourTreeNodeDecoratorState
    {
        private float m_time;

        public float time
        {
            get => m_time;
            set => m_time = value;
        }

        public TimeoutBehaviourTreeNodeDecoratorState()
        {
            m_time = 0.0f;
        }
    }
}
