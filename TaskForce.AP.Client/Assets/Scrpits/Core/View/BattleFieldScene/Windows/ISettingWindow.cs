using System;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.Windows
{
    public interface ISettingWindow : IWindow
    {
        event EventHandler<ValueChangedEventArgs> BGMVolumeChangedEvent;
        event EventHandler<ValueChangedEventArgs> SFXVolumeChangedEvent;
        event EventHandler LobbyButtonClickedEvent;
        event EventHandler ContinueButtonClickedEvent;

        void SetBGMVolume(float volume);
        void SetSFXVolume(float volume);
    }
}
