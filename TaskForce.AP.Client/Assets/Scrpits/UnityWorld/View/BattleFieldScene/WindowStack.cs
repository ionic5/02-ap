using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class WindowStack : View.WindowStack, IWindowStack
    {
        public Windows.SkillSelectionWindow SkillSelectionWindow;
        public Windows.SettingWindow SettingWindow;

        public ISettingWindow OpenSettingWindow()
        {
            return OpenWindow(SettingWindow);
        }

        public ISkillSelectionWindow OpenSkillSelectionWindow()
        {
            return OpenWindow(SkillSelectionWindow);
        }
    }
}