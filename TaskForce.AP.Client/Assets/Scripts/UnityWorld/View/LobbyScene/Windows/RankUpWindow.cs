using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.LobbyScene.Windows
{
    public class RankUpWindow : Window, IRankUpWindow
    {
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        public event EventHandler ConfirmButtonClickedEvent;
        public event EventHandler CancelButtonClickedEvent;

        public void OnClickConfirmButton()
        {
            ConfirmButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnClickCancelButton()
        {
            CancelButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetDescription(string text)
        {
            descriptionText.text = text;
        }
        
        public override void Clear()
        {
            base.Clear();

            ConfirmButtonClickedEvent = null;
            CancelButtonClickedEvent = null;
        }
    }
}
