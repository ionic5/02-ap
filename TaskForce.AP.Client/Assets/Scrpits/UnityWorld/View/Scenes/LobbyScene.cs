using System;
using TaskForce.AP.Client.Core.View.Scenes;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene;
using TaskForce.AP.Client.UnityWorld.View.LobbyScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.Scenes
{
    public class LobbyScene : Scene, ILobbyScene  
    {
        [SerializeField]
        private Loop _loop;
        [SerializeField]
        private LobbyWorld _lobbyWorld;
        [SerializeField]
        private View.BattleFieldScene.WindowStack _windowStack;
        [SerializeField]
        private PausePanel _pausePanel;
        public event EventHandler PauseButtonClickedEvent;
        public event EventHandler PlayButtonClickedEvent;

        public Loop Loop => _loop;
        public LobbyWorld LobbyWorld => _lobbyWorld;
        public View.BattleFieldScene.WindowStack WindowStack => _windowStack;
        public PausePanel PausePanel => _pausePanel;
        
        public void OnPauseButtonClicked()
        {
            PauseButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnPlayButtonClicked()
        {
            PlayButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void SetGold(int gold)
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
        
            PauseButtonClickedEvent = null;
            PlayButtonClickedEvent = null;
        }
    }
}
