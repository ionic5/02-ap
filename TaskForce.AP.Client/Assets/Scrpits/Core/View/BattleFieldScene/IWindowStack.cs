using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IWindowStack
    {
        ISettingWindow OpenSettingWindow();
        ISkillSelectionWindow OpenSkillSelectionWindow();
        int GetOpenedWindowCount();
    }
}
