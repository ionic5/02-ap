using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class EnergyGetWindowController
    {
        private readonly IEnergyGetWindow _window;
        private readonly Action _onEnergyGetConfirmed;

        public EnergyGetWindowController(IEnergyGetWindow window, Action onEnergyGetConfirmed)
        {
            _window = window;
            _onEnergyGetConfirmed = onEnergyGetConfirmed;
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

            _onEnergyGetConfirmed?.Invoke();    // TODO: JW: 광고 시청 후 에너지 지급 기능 추후 적용
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
