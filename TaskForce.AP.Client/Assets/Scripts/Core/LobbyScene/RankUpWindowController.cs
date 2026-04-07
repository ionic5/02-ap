using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class RankUpWindowController
    {
        private readonly IRankUpWindow _window;
        private readonly Action _onRankUpConfirmed;

        public RankUpWindowController(IRankUpWindow window, Action onRankUpConfirmed)
        {
            _window = window;
            _onRankUpConfirmed = onRankUpConfirmed;
        }

        public void Start()
        {
            _window.ConfirmButtonClickedEvent += OnConfirmButtonClicked;
            _window.CancelButtonClickedEvent += OnCancelButtonClicked;
        }

        private void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            _window.ConfirmButtonClickedEvent -= OnConfirmButtonClicked;
            _window.CancelButtonClickedEvent -= OnCancelButtonClicked;

            _onRankUpConfirmed?.Invoke();
            _window.Close();
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            _window.ConfirmButtonClickedEvent -= OnConfirmButtonClicked;
            _window.CancelButtonClickedEvent -= OnCancelButtonClicked;

            _window.Close();
        }
    }
}
