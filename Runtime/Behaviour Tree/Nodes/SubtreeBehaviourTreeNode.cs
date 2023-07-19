using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNode("Subtree")]
    public class SubtreeBehaviourTreeNode : ActionBehaviourTreeNode
    {
        [SerializeField]
        private BehaviourTree m_subtree;

        public override BehaviourTreeNodeState CreateNodeState(GameObject gameObject)
        {
            return new SubtreeBehaviourTreeNodeState(gameObject);
        }

        protected override BehaviourResult Execute(BehaviourTreeState state)
        {
            if (m_subtree != null)
            {
                SubtreeBehaviourTreeNodeState nodeState = state.GetNodeState(this) as SubtreeBehaviourTreeNodeState;
                return m_subtree.Run(nodeState.subtreeState);
            }

            return BehaviourResult.Failed;
        }
    }

    internal class SubtreeBehaviourTreeNodeState : BehaviourTreeNodeState
    {
        private BehaviourTreeState m_subtreeState;

        public BehaviourTreeState subtreeState => m_subtreeState;

        public SubtreeBehaviourTreeNodeState(GameObject gameObject)
        {
            m_subtreeState = new BehaviourTreeState(gameObject);
        }
    }
}
