namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface ISkillIconGrid
    {
        ISkillIcon AddIcon();
        ISkillIcon GetIcon(int index);
        bool IsIconExist(int index);
        void SetIconSlots(int count);
    }
}
