namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface ITargetIdentifier
    {
        bool IsEnemy(ITarget target);
        bool IsAlly(ITarget target);
    }
}
