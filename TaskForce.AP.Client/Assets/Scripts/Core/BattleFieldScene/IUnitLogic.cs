namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IUnitLogic : IDestroyable
    {
        void StartControl(IControllableUnit unit);
    }
}
