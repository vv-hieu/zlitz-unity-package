using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    public abstract class RandomizedCompositeBehaviourTreeNode : CompositeBehaviourTreeNode
    {
        [SerializeField]
        private RandomMode m_randomMode;

        private static System.Random m_rng;

        private static int randomInt
        {
            get
            {
                if (m_rng == null)
                {
                    m_rng = new System.Random();
                }
                return m_rng.Next();
            }
        }

        protected List<int> GetRandomOrder(BehaviourTreeState state)
        {
            int seed = m_randomMode == RandomMode.PerInstance ? state.gameObject.GetInstanceID() : randomInt;
            System.Random shuffleRng = new System.Random(seed);

            List<int>   indices = new List<int>();
            List<float> weights = new List<float>();

            float totalWeight = 0.0f;
            int index = 0;

            foreach (BehaviourTreeNode child in children)
            {
                float weight = Mathf.Max(0.0f, GetNodeWeight(child));
                totalWeight += weight;

                indices.Add(index++);
                weights.Add(weight);
            }

            List<int> order = new List<int>();
            while (weights.Count > 0)
            {
                int selected = Select(weights, totalWeight, shuffleRng);
                totalWeight -= weights[selected];

                order.Add(indices[selected]);

                indices.RemoveAt(selected);
                weights.RemoveAt(selected);
            }

            return order;
        }

        private int Select(List<float> weights, float totalWeight, System.Random rng)
        {
            float r = Mathf.Clamp((float)rng.NextDouble(), 0.0f, 0.999f) * totalWeight;
            float c = 0.0f;

            for (int i = 0; i < weights.Count; i++)
            {
                c += weights[i];
                if (c >= r)
                {
                    return i;
                }
            }

            return 0;
        }

        private float GetNodeWeight(BehaviourTreeNode node)
        {
            float result = 1.0f;

            IEnumerable<BehaviourTreeNodeDecorator> decorators = node.decorators;
            INodeWeightModifier[] weightModifiers = decorators == null ? null : decorators.OfType<INodeWeightModifier>().ToArray();

            if (weightModifiers != null)
            {
                for (int i = weightModifiers.Length - 1; i >= 0 ; i--)
                {
                    result = weightModifiers[i].Modify(result);
                }
            }

            return result;
        }
    
        public enum RandomMode
        {
            PerInstance,
            OnStart
        }
    }

    public interface INodeWeightModifier
    {
        float Modify(float nodeWeight);
    }
}
