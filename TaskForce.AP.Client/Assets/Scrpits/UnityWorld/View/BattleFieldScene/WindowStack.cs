using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class WindowStack : View.WindowStack, IWindowStack
    {
        public Windows.SkillSelectionWindow SkillSelectionWindow;
        public Windows.SettingWindow SettingWindow;
        public Windows.DeathWindow deathWindow;

        public IDeathWindow OpenDeathWindow()
        {
            return OpenWindow(deathWindow);
        }

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