using System;

namespace TaskForce.AP.Client.Core.View.Windows
{
    public interface ISettingWindow : IWindow
    {
        event EventHandler<ValueChangedEventArgs> BGMVolumeChangedEvent;
        event EventHandler<ValueChangedEventArgs> SFXVolumeChangedEvent;
        event EventHandler LobbyButtonClickedEvent;
        event EventHandler ContinueButtonClickedEvent;
        event EventHandler ConfirmButtonClickedEvent;

        void SetBGMVolume(float volume);
        void SetSFXVolume(float volume);
        void SetLobbyButtonVisible(bool visible);
        void SetContinueButtonVisible(bool visible);
        void SetConfirmButtonVisible(bool visible);
    }
}
