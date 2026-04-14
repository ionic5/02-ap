using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class StageHost : IUpdatable
    {
        private readonly View.BattleFieldScene.IWorld _world;
        private readonly GameDataStore _gameDataStore;
        private readonly Core.Timer _stageTimer;
        private readonly Core.ILogger _logger;
        private readonly Core.Random _random;
        private readonly Func<string, int, System.Numerics.Vector2, Unit> _createUnit;
        private IReadOnlyList<StageEnemy> _stageEnemies;
        private int _maxEnemyCount;
        private int _aliveEnemyCount;
        private int _stageLevel;

        public event EventHandler<DiedEventArgs> EnemyKilledEvent;

        public int GetStageLevel() => _stageLevel;

        public StageHost(View.BattleFieldScene.IWorld world, GameDataStore gameDataStore,
            Timer stageTimer,
            ILogger logger, Random random, Func<string, int, System.Numerics.Vector2, Unit> createUnit)
        {
            _world = world;
            _gameDataStore = gameDataStore;
            _stageTimer = stageTimer;
            _logger = logger;
            _random = random;
            _createUnit = createUnit;
        }

        public void Start(int stageLevel)
        {
            _logger.Info($"Stage(level:{stageLevel}) started.");

            _stageLevel = stageLevel;

            var stage = GetStage(stageLevel);
            _stageTimer.Start(stage.Time, OnStageFinished);
            _stageEnemies = _gameDataStore.GetStageEnemies().Where(entry => entry.StageLevel == stageLevel).ToList();
            _maxEnemyCount = stage.MaxEnemyCount;
        }

        public void Update()
        {
            if (!_stageTimer.IsRunning())
                return;
            if (_aliveEnemyCount >= _maxEnemyCount)
                return;

            SpawnEnemy();
        }

        private Stage GetStage(int stageLevel)
        {
            var stage = _gameDataStore.GetStages().Where(entry => entry.Level == stageLevel).FirstOrDefault();
            if (stage == null)
                _logger.Fatal($"Stage not exist for {stageLevel}");
            return stage;
        }

        private bool IsStageExist(int stageLevel)
        {
            return _gameDataStore.GetStages().Any(entry => entry.Level == stageLevel);
        }

        private void OnStageFinished()
        {
            var nextLevel = _stageLevel + 1;
            if (IsStageExist(nextLevel))
                Start(nextLevel);
            else
                _stageTimer.Stop();
        }

        private void SpawnEnemy()
        {
            var mob = SelectBySpawnRate();
            var unit = _createUnit(mob.UnitID, mob.Level, _world.GetNextSpawnPoint());

            _aliveEnemyCount++;

            EventHandler<DiedEventArgs> hdlr = null;
            hdlr = (sender, args) =>
            {
                unit.DiedEvent -= hdlr;
                _aliveEnemyCount--;
                EnemyKilledEvent?.Invoke(this, args);
            };
            unit.DiedEvent += hdlr;
        }

        private StageEnemy SelectBySpawnRate()
        {
            var totalRate = _stageEnemies.Sum(e => e.SpawnRate);
            var roll = _random.Next(0f, totalRate);
            var accumulated = 0f;
            foreach (var entry in _stageEnemies)
            {
                accumulated += entry.SpawnRate;
                if (roll < accumulated)
                    return entry;
            }

            return _stageEnemies[_stageEnemies.Count - 1];
        }
    }
}
