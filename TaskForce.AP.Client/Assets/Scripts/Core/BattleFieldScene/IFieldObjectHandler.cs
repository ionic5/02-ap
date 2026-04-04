namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IFieldObjectHandler
    {
        void Handle(ExpOrb orb);
        void Handle(IFieldItem item);
        void Handle(RootBox box);
    }
}
