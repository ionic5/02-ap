namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IFieldItem : IFieldObject
    {
        void Handle(IFieldItemHandler handler);
    }
}
