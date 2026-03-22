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
        private readonly Func<Entity.IActiveSkill, Skills.ISkill> _createSkill;
        private readonly Func<string, ISkill> _createSkillEntity;
        private readonly Func<IControllableUnit, string, IUnitLogic> _createUnitLogic;

        public UnitFactory(Random random, Func<Timer> createTimer, ITargetFinder targetFinder,
            Func<string, View.BattleFieldScene.IUnit> createUnitView,
            ILogger logger, Func<Entity.IActiveSkill, Skills.ISkill> createSkill, GameDataStore gameDataStore,
            Func<IControllableUnit, string, IUnitLogic> createUnitLogic, Func<string, ISkill> createSkillEntity)
        {
            _random = random;
            _createTimer = createTimer;
            _targetFinder = targetFinder;
            _createUnitView = createUnitView;
            _logger = logger;
            _createSkill = createSkill;
            _createSkillEntity = createSkillEntity;
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

            var entity = CreateUnitEntity(unitID, level);
            entity.SetPlayerSide(true);

            var unit = Create(entity);
            unit.SetUnitLogic(gdNonPlayerUnitLogic.UnitLogicID);

            return unit;
        }

        public IUnit CreateEnemyUnit(string unitID, int level)
        {
            var gdNonPlayerUnitLogic = _gameDataStore.GetNonPlayerUnitLogics().FirstOrDefault(entry => entry.UnitID == unitID);
            if (gdNonPlayerUnitLogic == null)
            {
                _logger.Fatal($"Failed to find unit id ({unitID}) on non player unit logic table.");
                return null;
            }

            var entity = CreateUnitEntity(unitID, level);
            entity.SetPlayerSide(false);

            var unit = Create(entity);
            unit.SetUnitLogic(gdNonPlayerUnitLogic.UnitLogicID);

            return unit;
        }

        private Entity.Unit CreateUnitEntity(string unitID, int level)
        {
            var gdUnit = _gameDataStore.GetUnits().FirstOrDefault(entry => entry.ID == unitID);
            if (gdUnit == null)
            {
                _logger.Fatal($"Failed to find unit in game data for unit id ({unitID}).");
                return null;
            }

            var entity = new Entity.Unit(_gameDataStore, _gameDataStore.GetBaseAttributes(gdUnit.BaseAttributeID), _gameDataStore.GetLevelAttributes(gdUnit.LevelAttributeID));

            entity.SetLevel(level);

            entity.SetHP(entity.GetAttribute(AttributeID.MaxHP).AsInt());

            var gdSkill = _gameDataStore.GetUnitDefaultSkills().FirstOrDefault(entry => entry.UnitID == unitID);
            if (gdSkill != null)
            {
                var skill = _createSkillEntity.Invoke(gdSkill.SkillID);
                skill.SetOwner(entity);
                skill.AddToOwner();

                entity.SetDefaultSkill(skill.GetSkillID());
            }
            else
            {
                _logger.Info($"Failed to find default skill for unit id. ({unitID})");
            }

            return entity;
        }
    }
}
