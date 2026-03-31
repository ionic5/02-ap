using System;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class SettingWindowController
    {
        private readonly ISettingWindow _window;
        private readonly Action _onGoToLobby;
        private readonly Core.SettingWindowController _commonCtrl;

        public SettingWindowController(ISettingWindow window, Core.SettingWindowController commonCtrl, Action onGoToLobby)
        {
            _window = window;
            _onGoToLobby = onGoToLobby;
            _commonCtrl = commonCtrl;
        }

        public void Start()
        {
            _commonCtrl.Start();

            _window.SetLobbyButtonVisible(true);
            _window.SetContinueButtonVisible(true);
            _window.SetConfirmButtonVisible(false);

            _window.LobbyButtonClickedEvent += OnLobbyButtonClicked;
            _window.ContinueButtonClickedEvent += OnContinueButtonClicked;
        }

        private void OnLobbyButtonClicked(object sender, EventArgs e)
        {
            _window.Close();
            _onGoToLobby?.Invoke();
        }

        private void OnContinueButtonClicked(object sender, EventArgs e)
        {
            _window.Close();
        }
    }
}
