using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.Windows
{
    public class SettingWindow : Window, ISettingWindow
    {
        [SerializeField]
        private Slider _bgmVolumeSlider;
        [SerializeField]
        private Slider _sfxVolumeSlider;

        public event EventHandler<ValueChangedEventArgs> BGMVolumeChangedEvent;
        public event EventHandler<ValueChangedEventArgs> SFXVolumeChangedEvent;
        public event EventHandler LobbyButtonClickedEvent;
        public event EventHandler ContinueButtonClickedEvent;

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

        public void SetBGMVolume(float volume)
        {
            _bgmVolumeSlider.value = volume;
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolumeSlider.value = volume;
        }

        public override void Clear()
        {
            base.Clear();

            BGMVolumeChangedEvent = null;
            SFXVolumeChangedEvent = null;
            LobbyButtonClickedEvent = null;
            ContinueButtonClickedEvent = null;
        }
    }
}
