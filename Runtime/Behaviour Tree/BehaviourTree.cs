using System.Collections.Generic;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [CreateAssetMenu(menuName = "Zlitz/AI/Behaviour Trees/Behaviour Tree")]
    public class BehaviourTree : ScriptableObject
    {
        [SerializeField]
        private BehaviourTreeNode m_rootNode;

        public BehaviourResult Run(BehaviourTreeState state)
        {
            if (m_rootNode != null)
            {
                if (!state.running)
                {
                    state.running = true;
                }

                BehaviourResult result = m_rootNode.Run(state);

                if (result != BehaviourResult.Running)
                {
                    state.running = false;
                    state.Reset();
                }

                return result;
            }

            return BehaviourResult.Failed;
        }
    }

    public class BehaviourTreeState
    {
        private Dictionary<string, BehaviourTreeNodeState>          m_nodeStates;
        private Dictionary<string, BehaviourTreeNodeDecoratorState> m_decoratorStates;

        private GameObject m_gameObject;

        private bool m_running;

        public bool running
        {
            get          => m_running;
            internal set => m_running = value;
        }

        public GameObject gameObject => m_gameObject;

        public BehaviourTreeNodeState GetNodeState(BehaviourTreeNode node)
        {
            if (!m_nodeStates.TryGetValue(node.guid, out BehaviourTreeNodeState state))
            {
                state = node.CreateNodeState(m_gameObject);
                m_nodeStates.Add(node.guid, state);
            }
            return state;
        }

        public BehaviourTreeNodeDecoratorState GetDecoratorState(BehaviourTreeNodeDecorator decorator)
        {
            if (!m_decoratorStates.TryGetValue(decorator.guid, out BehaviourTreeNodeDecoratorState state))
            {
                state = decorator.CreateDecoratorState();
                m_decoratorStates.Add(decorator.guid, state);
            }
            return state;
        }

        public void Reset()
        {
            m_nodeStates.Clear();
            m_decoratorStates.Clear();
        }

        public BehaviourTreeState(GameObject gameObject)
        {
            m_nodeStates      = new Dictionary<string, BehaviourTreeNodeState>();
            m_decoratorStates = new Dictionary<string, BehaviourTreeNodeDecoratorState>();

            m_gameObject = gameObject;
            m_running    = false;
        }
    }
}
