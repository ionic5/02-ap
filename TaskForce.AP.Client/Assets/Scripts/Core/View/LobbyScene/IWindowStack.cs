using TaskForce.AP.Client.Core.View.Windows;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.Core.View.LobbyScene

{
    public interface IWindowStack
    {
        IEnergyGetWindow OpenEnergyGetWindow();
        ICommonWindow OpenCommonWindow();
        IRankUpWindow OpenRankUpWindow();
        
        ISettingWindow OpenSettingWindow();
        
        int GetOpenedWindowCount();
    }
}