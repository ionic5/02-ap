using System;

namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface ILobbyScene : IDestroyable
    {
        event EventHandler PauseButtonClickedEvent;
        
        event EventHandler PlayButtonClickedEvent;
        event Action<int, int> UpdateUserDataStoreEvent;

        void LobbySceneControllerStarted();
        void SetGold(int gold);
        void SetEnergy(int energy);
    }
}