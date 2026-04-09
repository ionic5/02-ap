using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class EnergyGetWindowController
    {
        private readonly IEnergyGetWindow _window;
        private readonly Action _onEnergyGetConfirmed;
        private readonly IAdvertisementPlayer _advertisementPlayer;
        
        public EnergyGetWindowController(IEnergyGetWindow window, Action onEnergyGetConfirmed, IAdvertisementPlayer advertisementPlayer)
        {
            _window = window;
            _onEnergyGetConfirmed = onEnergyGetConfirmed;
            _advertisementPlayer = advertisementPlayer;
        }

        public void Start()
        {
            _window.ConfirmButtonClickedEvent += OnConfirmButtonClicked;
            _window.CancelButtonClickedEvent += OnCancelButtonClicked;
        }
        
        private void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            if (!_advertisementPlayer.CanPlayRewardedAdvertisement())
                return;
            
            _advertisementPlayer.PlayRewardedAdvertisement(() => { GetEnergyAndClose(); }, null);
        }

        private void GetEnergyAndClose()
        {
            _window.ConfirmButtonClickedEvent -= OnConfirmButtonClicked;
            _window.CancelButtonClickedEvent -= OnCancelButtonClicked;
            
            _onEnergyGetConfirmed?.Invoke();  
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
