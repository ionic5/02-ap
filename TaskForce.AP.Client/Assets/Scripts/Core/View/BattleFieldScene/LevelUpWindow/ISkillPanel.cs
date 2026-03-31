using System;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.LevelUpWindow
{
    public interface ISkillPanel
    {
        event EventHandler<SkillPanelClickedEventArgs> ClickedEvent;
        ISkillIcon GetIcon();
        void SetDescription(string text);
        void SetName(string text);
        void SetActiveNewMark(bool active);
        void SetSelected(bool isSelected);
    }
}
