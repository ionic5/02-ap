using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class EnergyGetWindowController
    {
        private readonly IEnergyGetWindow _window;

        public EnergyGetWindowController(IEnergyGetWindow window)
        {
            _window = window;
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
            
            _window.EnergyGetCompleted();   // TODO: JW: 광고 시청 후 에너지 지급 기능 추후 적용
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            _window.ConfirmButtonClickedEvent -= OnConfirmButtonClicked;
            _window.CancelButtonClickedEvent -= OnCancelButtonClicked;
            
            _window.Close();
        }
    }
}