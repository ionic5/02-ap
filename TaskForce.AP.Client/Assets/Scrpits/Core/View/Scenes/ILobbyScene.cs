using System;

namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface ILobbyScene : IDestroyable
    {
        event EventHandler PauseButtonClickedEvent;
        
        void SetGold(int gold);
    }
}