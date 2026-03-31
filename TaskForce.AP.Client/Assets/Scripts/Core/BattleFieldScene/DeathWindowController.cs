using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class DeathWindowController
    {
        private readonly IDeathWindow _window;
        private readonly int _level;
        private readonly int _kills;
        private readonly float _survivalTime;
        private readonly Action _onRestart;
        private readonly Action _onRevive;

        public DeathWindowController(IDeathWindow window, int level, int kills, float survivalTime, Action onRestart, Action onRevive)
        {
            _window = window;
            _level = level;
            _kills = kills;
            _survivalTime = survivalTime;
            _onRestart = onRestart;
            _onRevive = onRevive;
        }

        public void Start()
        {
            _window.SetLevelText(_level);
            _window.SetKillsText(_kills);
            _window.SetSurvivalTimeText(_survivalTime);

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