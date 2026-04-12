using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class BossStageHost
    {
        private readonly View.BattleFieldScene.IWorld _world;
        private readonly GameDataStore _gameDataStore;
        private readonly Timer _bossSpawnTimer;
        private readonly ILogger _logger;
        private readonly Func<string, int, System.Numerics.Vector2, Unit> _createUnit;

        public event EventHandler<BossStageClearedEventArgs> BossStageClearedEvent;

        private int _bossStageLevel;
        private int _aliveCount;

        public BossStageHost(View.BattleFieldScene.IWorld world, GameDataStore gameDataStore,
            Timer bossSpawnTimer, ILogger logger, Func<string, int, System.Numerics.Vector2, Unit> createUnit)
        {
            _world = world;
            _gameDataStore = gameDataStore;
            _bossSpawnTimer = bossSpawnTimer;
            _logger = logger;
            _createUnit = createUnit;
        }

        public void Start(int bossStageLevel)
        {
            _bossStageLevel = bossStageLevel;

            var bossStage = _gameDataStore.GetBossStage(bossStageLevel);
            if (bossStage == null)
            {
                _logger.Fatal($"BossStage not exist for level {bossStageLevel}");
                return;
            }

            _bossSpawnTimer.Start(bossStage.BossSpawnDelay, OnBossSpawnTimerElapsed);
        }

        private void OnBossSpawnTimerElapsed()
        {
            var enemies = _gameDataStore.GetBossStageEnemies(_bossStageLevel);

            foreach (var enemy in enemies)
            {
                for (var i = 0; i < enemy.Count; i++)
                {
                    var unit = _createUnit(enemy.UnitID, 1, _world.GetWarpPoint());

                    _aliveCount++;

                    EventHandler<DiedEventArgs> hdlr = null;
                    hdlr = (sender, args) =>
                    {
                        unit.DiedEvent -= hdlr;
                        _aliveCount--;
                        if (_aliveCount == 0)
                            BossStageClearedEvent?.Invoke(this, new BossStageClearedEventArgs(_bossStageLevel, args.DiedTarget.GetPosition()));
                    };
                    unit.DiedEvent += hdlr;
                }
            }

            _logger.Info($"BossStage(level:{_bossStageLevel}) enemies spawned.");
        }
    }
}
