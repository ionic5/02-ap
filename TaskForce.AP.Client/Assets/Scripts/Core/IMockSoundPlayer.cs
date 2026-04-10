namespace TaskForce.AP.Client.Core
{
    public interface IMockSoundPlayer
    {
        float GetBGMVolume();
        void SetBGMVolume(float volume);
        float GetSFXVolume();
        void SetSFXVolume(float volume);
    }
}
