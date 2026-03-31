using TaskForce.AP.Client.Core.View.LobbyScene.Windows;
using TaskForce.AP.Client.Core.View.Windows;
using TaskForce.AP.Client.UnityWorld.View.LobbyScene.Windows;
using TaskForce.AP.Client.UnityWorld.View.Windows;
using IWindowStack = TaskForce.AP.Client.Core.View.LobbyScene.IWindowStack;

namespace TaskForce.AP.Client.UnityWorld.View.LobbyScene
{
    public class WindowStack : View.WindowStack, IWindowStack
    {
        public EnergyGetWindow energyGetWindow;
        public CommonWindow commonWindow;
        public RankUpWindow rankUpWindow;
        public SettingWindow settingWindow;
        
        public IEnergyGetWindow OpenEnergyGetWindow()
        {
            return OpenWindow(energyGetWindow);
        }

        public ICommonWindow OpenCommonWindow()
        {
            return OpenWindow(commonWindow);
        }

        public IRankUpWindow OpenRankUpWindow()
        {
            return OpenWindow(rankUpWindow);
        }
        
        public ISettingWindow OpenSettingWindow()
        {
            return OpenWindow(settingWindow);
        }
    }
}