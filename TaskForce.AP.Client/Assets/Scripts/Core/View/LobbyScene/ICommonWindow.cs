using System;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core.View.LobbyScene.Windows
{
    public interface ICommonWindow : IWindow
    {
        event EventHandler ConfirmButtonClickedEvent;

        void SetContentsText(string text);
    }
}
