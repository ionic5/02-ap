using System;
using System.Collections.Generic;
using System.Linq;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class UnitFactory
    {
        public event EventHandler<CreatedEventArgs<Unit>> UnitCreatedEvent;

        private readonly Core.Random _random;
        private readonly Func<Core.Timer> _createTimer;
        private readonly ITargetFinder _targetFinder;
        private readonly Func<string, View.BattleFieldScene.IUnit> _createUnitView;
        private readonly Core.ILogger _logger;
        private readonly GameDataStore _gameDataStore;
        private readonly Func<Entity.ISkill, Skills.ISkill> _createSkill;
        private readonly Func<string, int, Entity.Unit> _createUnitEntity;
        private readonly Func<IControllableUnit, string, IUnitLogic> _createUnitLogic;

        public UnitFactory(Random random, Func<Timer> createTimer, ITargetFinder targetFinder,
            Func<string, View.BattleFieldScene.IUnit> createUnitView,
            ILogger logger, Func<Entity.ISkill, Skills.ISkill> createSkill, GameDataStore gameDataStore,
            Func<IControllableUnit, string, IUnitLogic> createUnitLogic, Func<string, int, Entity.Unit> createUnitEntity)
        {
            _random = random;
            _createTimer = createTimer;
            _targetFinder = targetFinder;
            _createUnitView = createUnitView;
            _logger = logger;
            _createSkill = createSkill;
            _createUnitEntity = createUnitEntity;
            _gameDataStore = gameDataStore;
            _createUnitLogic = createUnitLogic;
        }

        public Unit Create(Entity.Unit unitEntity)
        {
            var unitView = _createUnitView(unitEntity.GetUnitBodyID());

            ITargetIdentifier enemyIdentifier = null;
            if (unitEntity.IsPlayerSide())
                enemyIdentifier = new PlayerEnemyIdentifier();
            else
                enemyIdentifier = new NonPlayerEnemyIdentifier();

            var timer = _createTimer();
            var unit = new Unit(unitView, unitEntity,
                _targetFinder, enemyIdentifier, _logger,
                _createSkill, _createUnitLogic);

            UnitCreatedEvent?.Invoke(this, new CreatedEventArgs<Unit>(unit));

            foreach (var skill in unitEntity.GetSkills())
                unit.AddSkill(skill);

            return unit;
        }

        public IUnit CreatePlayerUnit(Entity.Unit entity)
        {
            //var entity = CreateUnitEntity(unitID, 1);
            entity.SetPlayerSide(true);

            var unit = Create(entity);
            unit.SetUnitLogic("PLAYER");
            unit.SetHPBarVisible(true);

            return unit;
        }

        public IUnit CreateNonPlayerUnit(string unitID, int level)
        {
            var gdNonPlayerUnitLogic = _gameDataStore.GetNonPlayerUnitLogics().FirstOrDefault(entry => entry.UnitID == unitID);
            if (gdNonPlayerUnitLogic == null)
            {
                _logger.Fatal($"Failed to find unit id ({unitID}) on non player unit logic table.");
                return null;
            }

            var entity = _createUnitEntity(unitID, level);
            entity.SetPlayerSide(true);

            var unit = Create(entity);
            unit.SetUnitLogic(gdNonPlayerUnitLogic.UnitLogicID);
            unit.SetHPBarVisible(false);

            return unit;
        }

        public Unit CreateEnemyUnit(string unitID, int level, System.Numerics.Vector2 position)
        {
            var gdNonPlayerUnitLogic = _gameDataStore.GetNonPlayerUnitLogics().FirstOrDefault(entry => entry.UnitID == unitID);
            if (gdNonPlayerUnitLogic == null)
            {
                _logger.Fatal($"Failed to find unit id ({unitID}) on non player unit logic table.");
                return null;
            }

            var entity = _createUnitEntity(unitID, level);
            entity.SetPlayerSide(false);

            var unit = Create(entity);
            unit.SetUnitLogic(gdNonPlayerUnitLogic.UnitLogicID);
            unit.SetHPBarVisible(false);
            unit.SetPosition(position);
            return unit;
        }

    }
}
