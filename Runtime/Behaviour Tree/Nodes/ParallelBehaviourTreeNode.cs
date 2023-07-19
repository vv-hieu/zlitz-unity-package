using System.Collections.Generic;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNode("Parallel")]
    public class ParallelBehaviourTreeNode : CompositeBehaviourTreeNode
    {
        public override BehaviourTreeNodeState CreateNodeState(GameObject gameObject)
        {
            return new ParallelBehaviourTreeNodeState();
        }

        protected override void OnStart(BehaviourTreeState state)
        {
            base.OnStart(state);

            ParallelBehaviourTreeNodeState nodeState = state.GetNodeState(this) as ParallelBehaviourTreeNodeState;
            nodeState.Reset();
        }

        protected override BehaviourResult Execute(BehaviourTreeState state)
        {
            ParallelBehaviourTreeNodeState nodeState = state.GetNodeState(this) as ParallelBehaviourTreeNodeState;

            bool isRunning = false;
            bool hasFailed = false;

            foreach (BehaviourTreeNode child in children)
            {
                if (nodeState.Contains(child))
                {
                    continue;
                }

                BehaviourResult childResult = child.Run(state);
                if (childResult == BehaviourResult.Running)
                {
                    isRunning = true;
                }
                else
                {
                    nodeState.Add(child);
                    if (childResult == BehaviourResult.Failed)
                    {
                        hasFailed = true;
                    }
                }
            }

            if (isRunning)
            {
                return BehaviourResult.Running;
            }

            return hasFailed ? BehaviourResult.Failed : BehaviourResult.Successful;
        }
    }

    internal class ParallelBehaviourTreeNodeState : BehaviourTreeNodeState
    {
        private HashSet<BehaviourTreeNode> m_finishedNodes;

        internal bool Contains(BehaviourTreeNode node)
        {
            if (m_finishedNodes != null)
            {
                return m_finishedNodes.Contains(node);
            }

            return false;
        }

        internal void Add(BehaviourTreeNode node)
        {
            if (m_finishedNodes != null)
            {
                m_finishedNodes.Add(node);
            }
        }

        internal void Reset()
        {
            if (m_finishedNodes == null)
            {
                m_finishedNodes = new HashSet<BehaviourTreeNode>();
            }
            m_finishedNodes.Clear();
        }

        public ParallelBehaviourTreeNodeState()
        {
            Reset();
        }
    }
}
