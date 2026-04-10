namespace TaskForce.AP.Client.Core
{
    public class MockSoundPlayer : IMockSoundPlayer
    {
        private readonly ISoundPlayer _soundPlayer;
        private readonly UserDataStore _userDataStore;
        private readonly ILogger _logger;

        public MockSoundPlayer(ISoundPlayer soundPlayer, UserDataStore userDataStore, ILogger logger)
        {
            _soundPlayer = soundPlayer;
            _userDataStore = userDataStore;
            _logger = logger;

            SetSoundPlayerBgmAndSfx();
        }

        void SetSoundPlayerBgmAndSfx()
        {
            _soundPlayer?.SetBGMVolume(_userDataStore.GetBgmVolume());
            _soundPlayer?.SetSFXVolume(_userDataStore.GetSfxVolume());
            _soundPlayer?.Play();
        }
        
        public float GetBGMVolume()
        {
            return _userDataStore.GetBgmVolume();
        }

        public float GetSFXVolume()
        {
            return _userDataStore.GetSfxVolume();
        }

        public void SetBGMVolume(float volume)
        {
            _userDataStore.SetBgmVolume(volume);
            _soundPlayer?.SetBGMVolume(volume);
        }

        public void SetSFXVolume(float volume)
        {
            _userDataStore.SetSfxVolume(volume);
            _soundPlayer?.SetSFXVolume(volume);
        }
    }
}
