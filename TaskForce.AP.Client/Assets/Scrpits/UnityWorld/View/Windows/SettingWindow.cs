using System;
using TaskForce.AP.Client.Core.View.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld.View.Windows
{
    public class SettingWindow : Window, ISettingWindow
    {
        [SerializeField]
        private Slider _bgmVolumeSlider;
        [SerializeField]
        private Slider _sfxVolumeSlider;
        [SerializeField]
        private GameObject _lobbyButton;
        [SerializeField]
        private GameObject _continueButton;
        [SerializeField]
        private GameObject _confirmButton;

        public event EventHandler<ValueChangedEventArgs> BGMVolumeChangedEvent;
        public event EventHandler<ValueChangedEventArgs> SFXVolumeChangedEvent;
        public event EventHandler LobbyButtonClickedEvent;
        public event EventHandler ContinueButtonClickedEvent;
        public event EventHandler ConfirmButtonClickedEvent;

        public void OnBgmVolumeChanged(float value)
        {
            BGMVolumeChangedEvent?.Invoke(this, new ValueChangedEventArgs() { Value = value });
        }

        public void OnSfxVolumeChanged(float value)
        {
            SFXVolumeChangedEvent?.Invoke(this, new ValueChangedEventArgs() { Value = value });
        }

        public void OnLobbyButtonClicked()
        {
            LobbyButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnContinueButtonClicked()
        {
            ContinueButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnConfirmButtonClicked()
        {
            ConfirmButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetBGMVolume(float volume)
        {
            _bgmVolumeSlider.value = volume;
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolumeSlider.value = volume;
        }

        public void SetLobbyButtonVisible(bool visible)
        {
            _lobbyButton.SetActive(visible);
        }

        public void SetContinueButtonVisible(bool visible)
        {
            _continueButton.SetActive(visible);
        }

        public void SetConfirmButtonVisible(bool visible)
        {
            _confirmButton.SetActive(visible);
        }

        public override void Clear()
        {
            base.Clear();

            BGMVolumeChangedEvent = null;
            SFXVolumeChangedEvent = null;
            LobbyButtonClickedEvent = null;
            ContinueButtonClickedEvent = null;
            ConfirmButtonClickedEvent = null;
        }
    }
}
