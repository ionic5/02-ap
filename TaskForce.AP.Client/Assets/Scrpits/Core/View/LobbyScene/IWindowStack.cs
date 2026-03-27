using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.Core.View.LobbyScene

{
    public interface IWindowStack
    {
        IEnergyGetWindow OpenEnergyGetWindow();
        ICommonWindow OpenCommonWindow();
        IRankUpWindow OpenRankUpWindow();
        
        int GetOpenedWindowCount();
    }
}