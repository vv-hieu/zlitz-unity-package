namespace Zlitz.AI.BehaviourTrees.Runtime
{
    [BehaviourTreeNodeDecorator("Invert Result")]
    public class InvertResultBehaviourTreeNodeDecorator : BehaviourTreeNodeDecorator
    {
        public override BehaviourResult Execute(BehaviourTreeState state, BehaviourResult nodeResult)
        {
            if (nodeResult == BehaviourResult.Successful)
            {
                return BehaviourResult.Failed;
            }
            if (nodeResult == BehaviourResult.Failed)
            {
                return BehaviourResult.Successful;
            }
            return BehaviourResult.Running;
        }

        public override string GetDecoratorDescription()
        {
            return "Invert result";
        }
    }
}
