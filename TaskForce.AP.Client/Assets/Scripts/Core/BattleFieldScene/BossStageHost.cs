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
        private readonly Action<string, int, System.Numerics.Vector2> _createUnit;

        private int _bossStageLevel;

        public BossStageHost(View.BattleFieldScene.IWorld world, GameDataStore gameDataStore,
            Timer bossSpawnTimer, ILogger logger, Action<string, int, System.Numerics.Vector2> createUnit)
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
                    _createUnit(enemy.UnitID, 1, _world.GetWarpPoint());
            }

            _logger.Info($"BossStage(level:{_bossStageLevel}) enemies spawned.");
        }
    }
}
