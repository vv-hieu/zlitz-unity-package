using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNode("Wait For")]
    public class WaitForBehaviourTreeNode : ActionBehaviourTreeNode
    {
        [SerializeField, Min(0.0f)]
        private float m_time = 1.0f;

        public override BehaviourTreeNodeState CreateNodeState(GameObject gameObject)
        {
            return new WaitForBehaviourTreeNodeState();
        }

        protected override void OnStart(BehaviourTreeState state)
        {
            base.OnStart(state);
            
            WaitForBehaviourTreeNodeState nodeState = state.GetNodeState(this) as WaitForBehaviourTreeNodeState;
            nodeState.Reset();
        }

        protected override BehaviourResult Execute(BehaviourTreeState state)
        {
            WaitForBehaviourTreeNodeState nodeState = state.GetNodeState(this) as WaitForBehaviourTreeNodeState;

            if (nodeState.Tick(Time.deltaTime, m_time))
            {
                return BehaviourResult.Successful;
            }

            return BehaviourResult.Running;
        }

        private class WaitForBehaviourTreeNodeState : BehaviourTreeNodeState
        {
            private float m_time = 0.0f;

            public bool Tick(float dt, float time)
            {
                m_time += dt;
                return m_time >= time;
            }

            public void Reset()
            {
                m_time = 0.0f;
            }
        }
    }
}
