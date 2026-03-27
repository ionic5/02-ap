using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.LobbyScene.Windows
{
    public class CommonWindow : Window, ICommonWindow
    {
        [SerializeField] private TextMeshProUGUI _contentsText;
        
        public event EventHandler ConfirmButtonClickedEvent;
        
        public void OnClickConfirmButton()
        {
            ConfirmButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void SetContentsText(string contents)
        {
            if (_contentsText != null)
                _contentsText.text = contents;
        }
        
        public override void Clear()
        {
            base.Clear();
        
            ConfirmButtonClickedEvent = null;
        }
    }
}