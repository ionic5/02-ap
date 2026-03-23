using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class MockSoundPlayer : ISoundPlayer
    {
        private float _bgmVolume = 1.0f;
        private float _sfxVolume = 1.0f;

        public float GetBGMVolume() => _bgmVolume;
        public float GetSFXVolume() => _sfxVolume;

        public void SetBGMVolume(float volume)
        {
            _bgmVolume = volume;
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = volume;
        }
    }
}
