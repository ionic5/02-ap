using System;

namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface ILobbyScene : IDestroyable
    {
        event EventHandler PauseButtonClickedEvent;
        
        event EventHandler PlayButtonClickedEvent;
        
        void SetGold(int gold);
    }
}