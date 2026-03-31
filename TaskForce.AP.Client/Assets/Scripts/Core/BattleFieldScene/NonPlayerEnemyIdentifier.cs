namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class NonPlayerEnemyIdentifier : ITargetIdentifier
    {
        public bool IsAlly(ITarget target)
        {
            return !target.IsPlayerSide();
        }

        public bool IsEnemy(ITarget target)
        {
            return target.IsPlayerSide();
        }

    }
}
