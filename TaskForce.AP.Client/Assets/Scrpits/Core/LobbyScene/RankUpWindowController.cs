using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class RankUpWindowController
    {
        private readonly IRankUpWindow _window;

        public RankUpWindowController(IRankUpWindow window)
        {
            _window = window;
        }

        public void Start()
        {
            _window.ConfirmButtonClickedEvent += OnConfirmButtonClicked;
            _window.CancelButtonClickedEvent += OnCancelButtonClicked;
        }
        
        private void OnConfirmButtonClicked(object sender, EventArgs e)
        {
            _window.RankUp();   // TODO: JW: 광고 시청 후 에너지 지급 기능 추후 적용
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            _window.Close();
        }
    }
}