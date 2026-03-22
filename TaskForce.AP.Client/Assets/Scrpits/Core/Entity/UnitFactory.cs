using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.Entity
{
    public class UnitFactory
    {
        private readonly Core.ILogger _logger;
        private readonly GameDataStore _gameDataStore;
        private readonly Func<string, Entity.ISkill> _createSkill;

        public UnitFactory(ILogger logger, GameDataStore gameDataStore, Func<string, ISkill> createSkill)
        {
            _logger = logger;
            _gameDataStore = gameDataStore;
            _createSkill = createSkill;
        }

        public Entity.Unit CreateUnitEntity(string unitID)
        {
            var gdUnit = _gameDataStore.GetUnits().FirstOrDefault(entry => entry.ID == unitID);
            if (gdUnit == null)
            {
                _logger.Fatal($"Failed to find unit in game data for unit id ({unitID}).");
                return null;
            }

            var entity = new Entity.Unit(_gameDataStore, _gameDataStore.GetBaseAttributes(gdUnit.BaseAttributeID), 
                _gameDataStore.GetLevelAttributes(gdUnit.LevelAttributeID));

            entity.SetLevel(1);

            entity.SetHP(entity.GetAttribute(AttributeID.MaxHP).AsInt());

            var skillDatas = _gameDataStore.GetUnitDefaultSkills().Where(entry => entry.UnitID == unitID);
            foreach (var data in skillDatas)
            {
                var skill = _createSkill.Invoke(data.SkillID);
                skill.SetOwner(entity);
                skill.AddToOwner();
            }

            var defaultSkillData = _gameDataStore.GetUnitDefaultActiveSkillByUnitID(unitID);
            if (defaultSkillData != null)
                entity.SetDefaultSkill(defaultSkillData.SkillID);
            else
                _logger.Info($"Failed to find default skill for unit id. ({unitID})");

            return entity;
        }
    }
}
