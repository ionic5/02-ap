namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IFollowCamera
    {
        void SetTarget(Core.BattleFieldScene.IFollowable _unit);
        void UnsetTarget();
    }
}
