using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TaskForce.AP.Client.Core.View.Windows;
using TaskForce.AP.Client.UnityWorld.View.Windows;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class WindowStack : View.WindowStack, IWindowStack
    {
        public Windows.LevelUpWindow LevelUpWindow;
        public SettingWindow SettingWindow;
        public Windows.DeathWindow deathWindow;

        public IDeathWindow OpenDeathWindow()
        {
            return OpenWindow(deathWindow);
        }

        public ISettingWindow OpenSettingWindow()
        {
            return OpenWindow(SettingWindow);
        }

        public ILevelUpWindow OpenLevelUpWindow()
        {
            return OpenWindow(LevelUpWindow);
        }
    }
}