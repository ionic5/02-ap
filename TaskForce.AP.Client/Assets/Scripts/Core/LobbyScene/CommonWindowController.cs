using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class CommonWindowController
    {
        private readonly ICommonWindow _window;

        public CommonWindowController(ICommonWindow window)
        {
            _window = window;
        }

        public void Start()
        {
            _window.ConfirmButtonClickedEvent += OnConfirmButtonClicked;
        }
        
        private void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            _window.Close();
        }
    }
}