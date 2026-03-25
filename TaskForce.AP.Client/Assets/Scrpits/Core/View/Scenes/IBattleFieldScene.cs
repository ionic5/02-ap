using System;

namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface IBattleFieldScene : IDestroyable
    {
        event EventHandler PauseButtonClickedEvent;

        void SetExp(int v);
        void SetLevel(string v);
        void SetRequireExp(int v);
        void SetKillCount(int v);
        void SetBattleTime(float v);
    }
}
