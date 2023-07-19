using UnityEngine;

using Zlitz.Utility.Serializables.Runtime;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNode("Event")]
    public class EventBehaviourTreeNode : ActionBehaviourTreeNode
    {
        [SerializeField]
        private ActionReference m_onStart;

        [SerializeField]
        private ActionReference m_onFinish;

        [SerializeField]
        private FunctionReference<BehaviourResult> m_execute;

        protected override void OnStart(BehaviourTreeState state)
        {
            base.OnStart(state);

            m_onStart?.Invoke(state.gameObject);
        }

        protected override void OnFinish(BehaviourTreeState state)
        {
            base.OnFinish(state);

            m_onFinish?.Invoke(state.gameObject);
        }

        protected override BehaviourResult Execute(BehaviourTreeState state)
        {
            if (m_execute != null)
            {
                try
                {
                    return m_execute.Invoke(state.gameObject);
                }
                catch
                {
                }
            }

            return BehaviourResult.Failed;
        }
    }
}
