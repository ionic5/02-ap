using System;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.LevelUpWindow
{
    public interface ISkillPanel
    {
        event EventHandler<SkillPanelClickedEventArgs> ClickedEvent;
        ISkillIcon GetIcon();
        void SetDescription(string v);
        void SetName(string v);
    }
}
