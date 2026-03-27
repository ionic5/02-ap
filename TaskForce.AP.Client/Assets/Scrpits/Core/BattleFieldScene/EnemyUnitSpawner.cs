using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class EnemyUnitSpawner : IUpdatable
    {
        private readonly View.BattleFieldScene.IWorld _world;
        private readonly GameDataStore _gameDataStore;
        private readonly Core.Timer _timer;
        private readonly Core.ILogger _logger;
        private readonly Core.Random _random;
        private readonly Func<string, int, IUnit> _createUnit;

        private List<IUnit> _activeEnemyUnits;
        private IReadOnlyList<StageEnemyUnit> _stageEnemyUnits;
        private int _maxEnemyUnitCount;
        private int _stageLevel;

        public EnemyUnitSpawner(View.BattleFieldScene.IWorld world, GameDataStore gameDataStore,
            Timer timer, ILogger logger, Random random, Func<string, int, IUnit> createUnit)
        {
            _world = world;
            _gameDataStore = gameDataStore;
            _activeEnemyUnits = new List<IUnit>();
            _timer = timer;
            _logger = logger;
            _random = random;
            _createUnit = createUnit;
        }

        public void Start(int stageLevel)
        {
            _logger.Info($"Stage(level:{stageLevel}) started.");

            _stageLevel = stageLevel;

            var stage = GetStage(stageLevel);
            _timer.Start(stage.Time, OnStageFinished);
            _stageEnemyUnits = _gameDataStore.GetStageEnemyUnits().Where(entry => entry.StageLevel == stageLevel).ToList();
            _maxEnemyUnitCount = stage.MaxEnemyUnitCount;
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
                _timer.Stop();
        }

        public void Update()
        {
            if (!_timer.IsRunning())
                return;

            if (_activeEnemyUnits.Count >= _maxEnemyUnitCount)
                return;

            var mob = _stageEnemyUnits[_random.Next(0, _stageEnemyUnits.Count)];
            Spawn(mob.UnitID, mob.Level);
        }

        private void Spawn(string unitID, int level)
        {
            _logger.Info($"Enemy unit spawned. UnitID : {unitID} Level : {level}");

            var unit = _createUnit.Invoke(unitID, level);

            _activeEnemyUnits.Add(unit);
            unit.DestroyEvent += OnDestroyUnitEvent;
            unit.DiedEvent += OnUnitDiedEvent;

            var spawnPos = _world.GetWarpPoint();
            unit.SetPosition(spawnPos);
        }

        private void OnUnitDiedEvent(object sender, DiedEventArgs e)
        {
            var unit = _activeEnemyUnits.FirstOrDefault(u => ReferenceEquals(u, e.DiedTarget));
            if (unit == null)
                return;
            RemoveUnit(unit);
        }

        private void OnDestroyUnitEvent(object sender, DestroyEventArgs e)
        {
            var unit = _activeEnemyUnits.FirstOrDefault(u => ReferenceEquals(u, e.TargetObject));
            if (unit == null)
                return;
            RemoveUnit(unit);
        }

        private void RemoveUnit(IUnit unit)
        {
            if (!_activeEnemyUnits.Remove(unit))
                return;

            unit.DiedEvent -= OnUnitDiedEvent;
            unit.DestroyEvent -= OnDestroyUnitEvent;
        }
    }
}
