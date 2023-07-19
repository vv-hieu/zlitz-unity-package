using System.Linq;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNode("Selector")]
    public class SelectorBehaviourTreeNode : CompositeBehaviourTreeNode
    {
        public override BehaviourTreeNodeState CreateNodeState(GameObject gameObject)
        {
            return new SelectorBehaviourTreeNodeState();
        }

        protected override void OnStart(BehaviourTreeState state)
        {
            base.OnStart(state);

            SelectorBehaviourTreeNodeState nodeState = state.GetNodeState(this) as SelectorBehaviourTreeNodeState;
            nodeState.currentIndex = 0;
        }

        protected override BehaviourResult Execute(BehaviourTreeState state)
        {
            if (children != null && children.GetEnumerator().MoveNext())
            {
                SelectorBehaviourTreeNodeState nodeState = state.GetNodeState(this) as SelectorBehaviourTreeNodeState;

                BehaviourTreeNode currentNode = children.ToArray()[nodeState.currentIndex];
                BehaviourResult currentNodeResult = currentNode.Run(state);

                if (currentNodeResult == BehaviourResult.Failed)
                {
                    nodeState.currentIndex++;
                    if (nodeState.currentIndex >= children.ToArray().Length)
                    {
                        return BehaviourResult.Failed;
                    }
                }
                if (currentNodeResult == BehaviourResult.Successful)
                {
                    return BehaviourResult.Successful;
                }
                return BehaviourResult.Running;
            }

            return BehaviourResult.Failed;
        }
    }

    internal class SelectorBehaviourTreeNodeState : BehaviourTreeNodeState
    {
        private int m_currentIndex;

        public int currentIndex
        {
            get => m_currentIndex;
            set => m_currentIndex = value;
        }

        public SelectorBehaviourTreeNodeState()
        {
            m_currentIndex = 0;
        }
    }
}
