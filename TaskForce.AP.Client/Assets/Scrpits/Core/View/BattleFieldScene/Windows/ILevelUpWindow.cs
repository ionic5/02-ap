using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.LevelUpWindow;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.Windows
{
    public interface ILevelUpWindow : IWindow
    {
        event EventHandler OKButtonClickedEvent;

        ISkillPanel AddSkillPanel();
        int GetSelectedSkillIndex();
    }
}
