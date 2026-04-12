using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class BossStageClearedEventArgs : EventArgs
    {
        public readonly int BossStageLevel;
        public readonly Vector2 LastBossDiedPosition;

        public BossStageClearedEventArgs(int bossStageLevel, Vector2 lastBossDiedPosition)
        {
            BossStageLevel = bossStageLevel;
            LastBossDiedPosition = lastBossDiedPosition;
        }
    }
}
