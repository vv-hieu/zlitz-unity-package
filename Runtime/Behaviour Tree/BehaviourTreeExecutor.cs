using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    public class BehaviourTreeExecutor : MonoBehaviour
    {
        [SerializeField]
        private BehaviourTree m_behaviourTree;

        private BehaviourTreeState m_state;

        public BehaviourTreeState state => m_state;

        void Start()
        {
            m_state = new BehaviourTreeState(gameObject);
        }

        void Update()
        {
            m_behaviourTree?.Run(m_state);
        }
    }
}
