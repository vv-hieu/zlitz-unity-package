using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNode("Randomized Sequence")]
    public class RandomizedSequenceBehaviourTreeNode : RandomizedCompositeBehaviourTreeNode
    {
        public override BehaviourTreeNodeState CreateNodeState(GameObject gameObject)
        {
            return new RandomizedSequenceBehaviourTreeNodeState();
        }

        protected override void OnStart(BehaviourTreeState state)
        {
            base.OnStart(state);

            RandomizedSequenceBehaviourTreeNodeState nodeState = state.GetNodeState(this) as RandomizedSequenceBehaviourTreeNodeState;
            nodeState.currentIndex = 0;
            nodeState.order        = GetRandomOrder(state);
        }

        protected override BehaviourResult Execute(BehaviourTreeState state)
        {
            if (children != null && children.GetEnumerator().MoveNext())
            {
                RandomizedSequenceBehaviourTreeNodeState nodeState = state.GetNodeState(this) as RandomizedSequenceBehaviourTreeNodeState;

                BehaviourTreeNode currentNode = children.ToArray()[nodeState.order[nodeState.currentIndex]];
                BehaviourResult currentNodeResult = currentNode.Run(state);

                if (currentNodeResult == BehaviourResult.Successful)
                {
                    nodeState.currentIndex++;
                    if (nodeState.currentIndex >= children.ToArray().Length)
                    {
                        return BehaviourResult.Successful;
                    }
                }
                if (currentNodeResult == BehaviourResult.Failed)
                {
                    return BehaviourResult.Failed;
                }
                return BehaviourResult.Running;
            }

            return BehaviourResult.Successful;
        }
    }

    internal class RandomizedSequenceBehaviourTreeNodeState : BehaviourTreeNodeState
    {
        private int m_currentIndex;

        private List<int> m_order;

        public int currentIndex
        {
            get => m_currentIndex;
            set => m_currentIndex = value;
        }

        public List<int> order
        {
            get => m_order;
            set => m_order = value;
        }

        public RandomizedSequenceBehaviourTreeNodeState()
        {
            m_currentIndex = 0;
        }
    }
}
