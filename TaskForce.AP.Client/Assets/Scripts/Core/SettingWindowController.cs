using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core
{
    public class SettingWindowController
    {
        private readonly ISettingWindow _window;
        private readonly IMockSoundPlayer _mockSoundPlayer;

        public SettingWindowController(ISettingWindow window, IMockSoundPlayer mockSoundPlayer)
        {
            _window = window;
            _mockSoundPlayer = mockSoundPlayer;
        }

        public void Start()
        {
            _window.SetBGMVolume(_mockSoundPlayer.GetBGMVolume());
            _window.SetSFXVolume(_mockSoundPlayer.GetSFXVolume());

            _window.BGMVolumeChangedEvent += OnBGMVolumeChanged;
            _window.SFXVolumeChangedEvent += OnSFXVolumeChanged;
        }

        private void OnBGMVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            _mockSoundPlayer.SetBGMVolume(e.Value);
        }

        private void OnSFXVolumeChanged(object sender, ValueChangedEventArgs e)
        {
            _mockSoundPlayer.SetSFXVolume(e.Value);
        }
    }
}
