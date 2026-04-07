using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.UnityWorld.View.LobbyScene.Windows
{
    public class RankUpWindow : Window, IRankUpWindow
    {
        public event EventHandler ConfirmButtonClickedEvent;
        public event EventHandler CancelButtonClickedEvent;

        public void OnClickConfirmButton()
        {
            ConfirmButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnClickCancelButton()
        {
            CancelButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public override void Clear()
        {
            base.Clear();

            ConfirmButtonClickedEvent = null;
            CancelButtonClickedEvent = null;
        }
    }
}
