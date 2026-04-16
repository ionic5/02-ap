namespace TaskForce.AP.Client.Core
{
    public interface ISoundPlayer
    {
        void SetBGMVolume(float volume);
        void SetSFXVolume(float volume);

        void PlayBgm();
        void PlayPlayerHitSfx();
    }
}