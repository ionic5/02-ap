using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class SettingWindowController
    {
        private readonly ISettingWindow _window;
        private readonly ISoundPlayer _soundPlayer;
        private readonly Action _onGoToLobby;

        public SettingWindowController(ISettingWindow window, ISoundPlayer soundPlayer, Action onGoToLobby = null)
        {
            _window = window;
            _soundPlayer = soundPlayer;
            _onGoToLobby = onGoToLobby;
        }

        public void Start()
        {
            _window.SetBGMVolume(_soundPlayer.GetBGMVolume());
            _window.SetSFXVolume(_soundPlayer.GetSFXVolume());

            _window.BGMVolumeChangedEvent += OnBGMVolumeChanged;
            _window.SFXVolumeChangedEvent += OnSFXVolumeChanged;
            _window.LobbyButtonClickedEvent += OnLobbyButtonClicked;
            _window.ContinueButtonClickedEvent += OnContinueButtonClicked;
        }

        private void OnBGMVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            _soundPlayer.SetBGMVolume(e.Value);
        }

        private void OnSFXVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            _soundPlayer.SetSFXVolume(e.Value);
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
