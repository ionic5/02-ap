using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class DeathWindowController
    {
        private readonly IDeathWindow _window;
        private readonly Action _onRestart;
        private readonly Action _onRevive;

        public DeathWindowController(IDeathWindow window, Action onRestart, Action onRevive)
        {
            _window = window;
            _onRestart = onRestart;
            _onRevive = onRevive;
        }

        public void Start()
        {
            _window.RestartClickedEvent += OnRestartClicked;
            _window.ReviveClickedEvent += OnReviveClicked;
        }

        private void OnRestartClicked(object sender, EventArgs e)
        {
            _window.RestartClickedEvent -= OnRestartClicked;
            _window.ReviveClickedEvent -= OnReviveClicked;
            _window.Close();
            _onRestart?.Invoke();
        }

        private void OnReviveClicked(object sender, EventArgs e)
        {
            _window.RestartClickedEvent -= OnRestartClicked;
            _window.ReviveClickedEvent -= OnReviveClicked;
            _window.Close();
            _onRevive?.Invoke();
        }
    }
}