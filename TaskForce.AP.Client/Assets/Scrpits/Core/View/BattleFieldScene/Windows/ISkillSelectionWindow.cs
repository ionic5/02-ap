using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.SkillSelectionWindow;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.Windows
{
    public interface ISkillSelectionWindow : IWindow
    {
        event EventHandler OKButtonClickedEvent;

        ISkillPanel AddSkillPanel();
        int GetSelectedSkillIndex();
    }
}
