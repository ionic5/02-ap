using System;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class SettingWindowController
    {
        private readonly ISettingWindow _window;
        private readonly Core.SettingWindowController _commonCtrl;

        public SettingWindowController(ISettingWindow window, Core.SettingWindowController commonCtrl)
        {
            _window = window;
            _commonCtrl = commonCtrl;
        }

        public void Start()
        {
            _commonCtrl.Start();

            _window.SetLobbyButtonVisible(false);
            _window.SetContinueButtonVisible(false);
            _window.SetConfirmButtonVisible(true);
            
            _window.ConfirmButtonClickedEvent += OnConfirmButtonClicked;
        }

        private void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            _window.Close();
        }
    }
}