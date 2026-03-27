using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core
{
    public class SettingWindowController
    {
        private readonly ISettingWindow _window;
        private readonly ISoundPlayer _soundPlayer;

        public SettingWindowController(ISettingWindow window, ISoundPlayer soundPlayer)
        {
            _window = window;
            _soundPlayer = soundPlayer;
        }

        public void Start()
        {
            _window.SetBGMVolume(_soundPlayer.GetBGMVolume());
            _window.SetSFXVolume(_soundPlayer.GetSFXVolume());

            _window.BGMVolumeChangedEvent += OnBGMVolumeChanged;
            _window.SFXVolumeChangedEvent += OnSFXVolumeChanged;
        }

        private void OnBGMVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            _soundPlayer.SetBGMVolume(e.Value);
        }

        private void OnSFXVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            _soundPlayer.SetSFXVolume(e.Value);
        }
    }
}
