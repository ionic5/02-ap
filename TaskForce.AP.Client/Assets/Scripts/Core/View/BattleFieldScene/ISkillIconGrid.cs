using System;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface ISkillIconGrid
    {
        event EventHandler DestroyedEvent;
        ISkillIcon AddIcon();
        ISkillIcon GetIcon(int index);
        bool IsIconExist(int index);
        void SetIconSlots(int count);
    }
}
