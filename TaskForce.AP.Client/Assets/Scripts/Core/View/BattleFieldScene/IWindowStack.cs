using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IWindowStack
    {
        event System.EventHandler WindowCountChangedEvent;
        ISettingWindow OpenSettingWindow();
        ILevelUpWindow OpenLevelUpWindow();
        IDeathWindow OpenDeathWindow();
        int GetOpenedWindowCount();
    }
}
