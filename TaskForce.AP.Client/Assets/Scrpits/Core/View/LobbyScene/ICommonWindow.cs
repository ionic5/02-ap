using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.View.LobbyScene.Windows
{
    public interface ICommonWindow : IWindow 
    {
        event EventHandler ConfirmButtonClickedEvent;
    }
}