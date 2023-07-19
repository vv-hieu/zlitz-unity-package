using UnityEngine;

namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNodeDecorator("Modify Weight")]
    public class ModifyWeightBehaviourTreeNodeDecorator : BehaviourTreeNodeDecorator, INodeWeightModifier
    {
        [SerializeField]
        private Operation m_opertion = Operation.Override;

        [SerializeField]
        private float m_amount = 1.0f;

        public override BehaviourResult Execute(BehaviourTreeState state, BehaviourResult nodeResult)
        {
            return nodeResult;
        }

        public override string GetDecoratorDescription()
        {
            string amount = $"{m_amount}";
            if (m_opertion == Operation.Add)
            {
                amount = "+" + amount;
            }
            else if (m_opertion == Operation.Multiply)
            {
                amount = "x" + amount;
            }
            return $"Weight {amount}";
        }

        public float Modify(float nodeWeight)
        {
            switch (m_opertion)
            {
                case Operation.Override:
                    return m_amount;
                case Operation.Add:
                    return nodeWeight + m_amount;
                case Operation.Multiply:
                    return nodeWeight * m_amount;
            }

            return nodeWeight;
        }

        public enum Operation
        {
            Override,
            Add,
            Multiply
        }
    }
}
