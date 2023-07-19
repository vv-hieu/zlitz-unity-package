using System;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    public abstract class BehaviourTreeNodeDecorator : ScriptableObject
    {
        [SerializeField]
        private string m_guid = "";

        public string guid => m_guid;

        public virtual BehaviourTreeNodeDecoratorState CreateDecoratorState()
        {
            return new BehaviourTreeNodeDecoratorState();
        }

        public virtual void OnStart(BehaviourTreeState state)
        {
        }

        public virtual void OnFinish(BehaviourTreeState state)
        {
        }

        public abstract BehaviourResult Execute(BehaviourTreeState state, BehaviourResult nodeResult);

        public BehaviourResult Run(BehaviourTreeState state, BehaviourResult nodeResult)
        {
            BehaviourTreeNodeDecoratorState decoratorState = state.GetDecoratorState(this);

            if (decoratorState.state == BehaviourTreeNodeDecoratorState.State.Idle)
            {
                decoratorState.state = BehaviourTreeNodeDecoratorState.State.Running;
                OnStart(state);
            }

            BehaviourResult result = Execute(state, nodeResult);

            if (result != BehaviourResult.Running)
            {
                decoratorState.state = result == BehaviourResult.Successful ? BehaviourTreeNodeDecoratorState.State.Successful : BehaviourTreeNodeDecoratorState.State.Failed;
                OnFinish(state);
            }

            return result;
        }

        public abstract string GetDecoratorDescription();

        public BehaviourTreeNodeDecorator()
        {
            if (m_guid == "")
            {
                m_guid = Guid.NewGuid().ToString();
            }
        }
    }

    public class BehaviourTreeNodeDecoratorState
    {
        private State m_state;

        public State state
        {
            get          => m_state;
            internal set => m_state = value;
        }

        public BehaviourTreeNodeDecoratorState()
        {
        }

        public enum State
        {
            Idle,
            Running,
            Successful,
            Failed
        }
    }
}
