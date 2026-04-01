using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class GameHost
    {
        private readonly View.BattleFieldScene.IWorld _world;
        private readonly GameDataStore _gameDataStore;
        private readonly Core.Timer _stageTimer;
        private readonly Core.Timer _spawnTimer;
        private readonly Core.Timer _swarmTimer;
        private readonly Core.ILogger _logger;
        private readonly Core.Random _random;
        private readonly Func<string, int, IUnit> _createUnit;

        private IReadOnlyList<StageEnemyUnit> _stageEnemyUnits;
        private float _spawnGap;
        private int _stageLevel;
        private GameData.EnemyUnitSwarm _swarmData;
        private float _swarmInterval;
        private bool _swarmInitialized;

        public GameHost(View.BattleFieldScene.IWorld world, GameDataStore gameDataStore,
            Timer stageTimer, Timer spawnTimer, Timer swarmTimer,
            ILogger logger, Random random, Func<string, int, IUnit> createUnit)
        {
            _world = world;
            _gameDataStore = gameDataStore;
            _stageTimer = stageTimer;
            _spawnTimer = spawnTimer;
            _swarmTimer = swarmTimer;
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
            _stageEnemyUnits = _gameDataStore.GetStageEnemyUnits().Where(entry => entry.StageLevel == stageLevel).ToList();
            _spawnGap = stage.SpawnGap;
            _spawnTimer.Start(_spawnGap, OnSpawnTimerElapsed);

            if (!_swarmInitialized)
            {
                _swarmData = _gameDataStore.GetEnemyUnitSwarms().FirstOrDefault();
                _swarmInterval = _gameDataStore.GetConstant(GameData.ConstantID.EnemyUnitSwarmInterval).AsFloat();
                _swarmTimer.Start(_swarmInterval, OnSwarmTimerElapsed);
                _swarmInitialized = true;
            }
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

        private void OnSpawnTimerElapsed()
        {
            if (!_stageTimer.IsRunning())
                return;
            var mob = SelectBySpawnRate();
            Spawn(mob.UnitID, mob.Level);
            _spawnTimer.Start(_spawnGap, OnSpawnTimerElapsed);
        }

        private void OnSwarmTimerElapsed()
        {
            SpawnSwarm();
            _swarmTimer.Start(_swarmInterval, OnSwarmTimerElapsed);
        }

        private StageEnemyUnit SelectBySpawnRate()
        {
            var totalRate = _stageEnemyUnits.Sum(e => e.SpawnRate);
            var roll = _random.Next(0f, totalRate);
            var accumulated = 0f;
            foreach (var entry in _stageEnemyUnits)
            {
                accumulated += entry.SpawnRate;
                if (roll < accumulated)
                    return entry;
            }
            return _stageEnemyUnits[_stageEnemyUnits.Count - 1];
        }

        private void SpawnSwarm()
        {
            var spawnPos = _world.GetWarpPoint();
            for (var i = 0; i < _swarmData.Count; i++)
                Spawn(_swarmData.UnitID, _swarmData.Level, spawnPos);
        }

        private void Spawn(string unitID, int level)
        {
            Spawn(unitID, level, _world.GetWarpPoint());
        }

        private void Spawn(string unitID, int level, System.Numerics.Vector2 spawnPos)
        {
            _logger.Info($"Enemy unit spawned. UnitID : {unitID} Level : {level}");

            var unit = _createUnit.Invoke(unitID, level);
            unit.SetPosition(spawnPos);
        }
    }
}
