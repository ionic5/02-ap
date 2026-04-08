namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IFieldItemHandler
    {
        void Handle(MedicalKit kit);
        void Handle(GoldBundle bundle);
        void Handle(Nuke nuke);
    }
}
