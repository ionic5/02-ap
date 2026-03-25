using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class BattleLogRecorder : IUpdatable
    {
        private readonly BattleLog _battleLog;
        private readonly ITime _time;

        public BattleLogRecorder(BattleLog battleLog, ITime time)
        {
            _battleLog = battleLog;
            _time = time;
        }

        public void Update()
        {
            _battleLog.AddBattleTime(_time.GetDeltaTime());
        }

        public void OnUnitDied(object sender, DiedEventArgs args)
        {
            if (args.Killer == null || !args.Killer.IsPlayerSide())
                return;

            _battleLog.AddKillCount();
        }
    }
}
