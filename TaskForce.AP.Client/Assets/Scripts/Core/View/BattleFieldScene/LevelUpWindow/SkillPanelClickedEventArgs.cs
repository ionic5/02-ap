using System;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.LevelUpWindow
{
    public class SkillPanelClickedEventArgs : EventArgs
    {
        public ISkillPanel Panel { get; }

        public SkillPanelClickedEventArgs(ISkillPanel panel)
        {
            Panel = panel;
        }
    }
}
