using System;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.Windows
{
    public interface IDeathWindow : IWindow
    {
        event EventHandler RestartClickedEvent;
        event EventHandler ReviveClickedEvent;

        void SetLevelText(int level);
        void SetKillsText(int kills);
        void SetSurvivalTimeText(float time);
    }
}