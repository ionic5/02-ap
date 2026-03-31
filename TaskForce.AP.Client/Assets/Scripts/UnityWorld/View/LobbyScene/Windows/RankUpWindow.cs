using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld.View.LobbyScene.Windows
{
    public class RankUpWindow : Window, IRankUpWindow
    {
        [SerializeField] private Scenes.LobbyScene _lobbyScene;
        
        public event EventHandler ConfirmButtonClickedEvent;
        public event EventHandler CancelButtonClickedEvent;
        
        public void RankUp()
        {
            _lobbyScene.RankUp();
            Close();
        }
        
        public void OnClickConfirmButton()
        {
            ConfirmButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnClickCancelButton()
        {
            CancelButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public override void Clear()
        {
            base.Clear();
        
            ConfirmButtonClickedEvent = null;
            CancelButtonClickedEvent = null;
        }
    }
}