using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNode("Randomized Selector")]
    public class RandomizedSelectorBehaviourTreeNode : RandomizedCompositeBehaviourTreeNode
    {
        public override BehaviourTreeNodeState CreateNodeState(GameObject gameObject)
        {
            return new RandomizedSelectorBehaviourTreeNodeState();
        }

        protected override void OnStart(BehaviourTreeState state)
        {
            base.OnStart(state);

            RandomizedSelectorBehaviourTreeNodeState nodeState = state.GetNodeState(this) as RandomizedSelectorBehaviourTreeNodeState;
            nodeState.currentIndex = 0;
            nodeState.order        = GetRandomOrder(state);

        }

        protected override BehaviourResult Execute(BehaviourTreeState state)
        {
            if (children != null && children.GetEnumerator().MoveNext())
            {
                RandomizedSelectorBehaviourTreeNodeState nodeState = state.GetNodeState(this) as RandomizedSelectorBehaviourTreeNodeState;
                
                BehaviourTreeNode currentNode = children.ToArray()[nodeState.order[nodeState.currentIndex]];
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

    internal class RandomizedSelectorBehaviourTreeNodeState : BehaviourTreeNodeState
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

        public RandomizedSelectorBehaviourTreeNodeState()
        {
            m_currentIndex = 0;
        }
    }
}
