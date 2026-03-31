using System;

namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface IBattleFieldScene : IDestroyable
    {
        event EventHandler PauseButtonClickedEvent;

        void SetExp(int exp);
        void SetLevel(string level);
        void SetRequireExp(int requireExp);
        void SetKillCount(int killCount);
        void SetBattleTime(float battleTime);
        void SetGold(int gold);
    }
}
