namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IFieldObject : IDestroyable, IPositionable
    {
        void Handle(IFieldObjectHandler handler);
    }
}
