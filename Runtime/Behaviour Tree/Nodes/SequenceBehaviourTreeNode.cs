using System.Linq;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNode("Sequence")]
    public class SequenceBehaviourTreeNode : CompositeBehaviourTreeNode
    {
        public override BehaviourTreeNodeState CreateNodeState(GameObject gameObject)
        {
            return new SequenceBehaviourTreeNodeState();
        }

        protected override void OnStart(BehaviourTreeState state)
        {
            base.OnStart(state);

            SequenceBehaviourTreeNodeState nodeState = state.GetNodeState(this) as SequenceBehaviourTreeNodeState;
            nodeState.currentIndex = 0;
        }

        protected override BehaviourResult Execute(BehaviourTreeState state)
        {
            if (children != null && children.GetEnumerator().MoveNext())
            {
                SequenceBehaviourTreeNodeState nodeState = state.GetNodeState(this) as SequenceBehaviourTreeNodeState;

                BehaviourTreeNode currentNode = children.ToArray()[nodeState.currentIndex];
                BehaviourResult currentNodeResult = currentNode.Run(state);

                if (currentNodeResult == BehaviourResult.Failed)
                {
                    return BehaviourResult.Failed;
                }
                if (currentNodeResult == BehaviourResult.Successful)
                {
                    nodeState.currentIndex++;
                    if (nodeState.currentIndex >= children.ToArray().Length)
                    {
                        return BehaviourResult.Successful;
                    }
                }
                return BehaviourResult.Running;
            }

            return BehaviourResult.Successful;
        }
    }

    internal class SequenceBehaviourTreeNodeState : BehaviourTreeNodeState
    {
        private int m_currentIndex;

        public int currentIndex
        {
            get => m_currentIndex;
            set => m_currentIndex = value;
        }

        public SequenceBehaviourTreeNodeState()
        {
            m_currentIndex = 0;
        }
    }
}
