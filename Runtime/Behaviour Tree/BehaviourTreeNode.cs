using System;
using System.Collections.Generic;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    public abstract class BehaviourTreeNode : ScriptableObject
    {
        [SerializeField]
        private string m_guid = "";

        [SerializeField]
        private List<BehaviourTreeNodeDecorator> m_decorators;

        public string guid => m_guid;

        public IEnumerable<BehaviourTreeNodeDecorator> decorators => m_decorators;

        public virtual BehaviourTreeNodeState CreateNodeState(GameObject gameObject)
        {
            return new BehaviourTreeNodeState();
        }

        protected virtual void OnStart(BehaviourTreeState state)
        {
        }

        protected virtual void OnFinish(BehaviourTreeState state)
        {
        }

        protected abstract BehaviourResult Execute(BehaviourTreeState state);

        public BehaviourResult Run(BehaviourTreeState state)
        {
            return RunWithDecorators(state, 0);
        }

        public BehaviourTreeNode()
        {
            if (m_guid == "")
            {
                m_guid = Guid.NewGuid().ToString();
            }
        }

        private BehaviourResult RunWithDecorators(BehaviourTreeState state, int decoratorIndex)
        {
            if (m_decorators == null || decoratorIndex >= m_decorators.Count)
            {
                BehaviourTreeNodeState nodeState = state.GetNodeState(this);

                if (nodeState.state == BehaviourTreeNodeState.State.Idle)
                {
                    nodeState.state = BehaviourTreeNodeState.State.Running;
                    OnStart(state);
                }

                BehaviourResult result = Execute(state);

                if (result != BehaviourResult.Running)
                {
                    nodeState.state = result == BehaviourResult.Successful ? BehaviourTreeNodeState.State.Successful : BehaviourTreeNodeState.State.Failed;
                    OnFinish(state);
                }

                return result;
            }

            BehaviourTreeNodeDecorator decorator = m_decorators[decoratorIndex];
            BehaviourResult nodeResult = RunWithDecorators(state, decoratorIndex + 1);
            return decorator.Run(state, nodeResult);
        }
    }

    public enum BehaviourResult
    {
        Running,
        Successful,
        Failed
    }

    public class BehaviourTreeNodeState
    {
        private State m_state;

        public State state
        {
            get          => m_state;
            internal set => m_state = value;
        }

        public BehaviourTreeNodeState()
        {
            m_state = State.Idle;
        }

        public enum State
        {
            Idle,
            Running,
            Successful,
            Failed
        }
    }

    public abstract class CompositeBehaviourTreeNode : BehaviourTreeNode
    {
        [SerializeField]
        private List<BehaviourTreeNode> m_children;

#pragma warning disable 0414
        [SerializeField]
        private bool m_collapsed = false;
#pragma warning restore 0414

        protected IEnumerable<BehaviourTreeNode> children => m_children;

        public CompositeBehaviourTreeNode() : base()
        {
            m_children = new List<BehaviourTreeNode>();
        }
    }

    public abstract class ActionBehaviourTreeNode : BehaviourTreeNode
    {
        public ActionBehaviourTreeNode() : base()
        {
        }
    }
}
