namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface ISoundPlayer
    {
        float GetBGMVolume();
        void SetBGMVolume(float volume);
        float GetSFXVolume();
        void SetSFXVolume(float volume);
    }
}
