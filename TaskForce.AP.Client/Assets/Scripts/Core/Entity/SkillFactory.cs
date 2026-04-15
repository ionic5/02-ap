using System.Linq;

namespace TaskForce.AP.Client.Core.Entity
{
    public class SkillFactory
    {
        private readonly GameDataStore _gameDataStore;
        private readonly TextStore _textStore;
        private readonly ILogger _logger;
        private readonly ModifyAttributeEffectFactory _modifyAttributeEffectFactory;

        public SkillFactory(GameDataStore gameDataStore, ILogger logger,
            TextStore textStore, ModifyAttributeEffectFactory modifyAttributeEffectFactory)
        {
            _gameDataStore = gameDataStore;
            _logger = logger;
            _textStore = textStore;
            _modifyAttributeEffectFactory = modifyAttributeEffectFactory;
        }

        public ISkill CreateSkill(string skillID)
        {
            var skillData = _gameDataStore.GetSkills().Where(entry => entry.ID == skillID).FirstOrDefault();
            if (skillData == null)
            {
                _logger.Fatal($"Failed to find skill data for {skillID}");
                return null;
            }

            var baseAttrs = _gameDataStore.GetSkillBaseAttributes(skillID);
            var lvAttrs = _gameDataStore.GetSkillLevelAttributes(skillID);
            var skillDescs = _gameDataStore.GetSkillDescriptions(skillID);

            ISkill skill;

            if (skillID == SkillID.MeleeAttack)
                skill = new Entity.MeleeAttackSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs);
            else if (skillID == SkillID.MeleeDagger)
                skill = new Entity.MeleeDaggerSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs);
            else if (skillID == SkillID.MeleeBat)
                skill = new Entity.MeleeBatSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs);
            else if (skillID == SkillID.Gloves)
                skill = new Entity.Equipment.GlovesSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs, _modifyAttributeEffectFactory);
            else if (skillID == SkillID.Armor)
                skill = new Entity.Equipment.ArmorSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs, _modifyAttributeEffectFactory);
            else if (skillID == SkillID.Helmet)
                skill = new Entity.Equipment.HelmetSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs, _modifyAttributeEffectFactory);
            else if (skillID == SkillID.Boots)
                skill = new Entity.Equipment.BootsSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs, _modifyAttributeEffectFactory);
            else if (skillID == SkillID.TacticalBackpack)
                skill = new Entity.Equipment.TacticalBackpackSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs, _modifyAttributeEffectFactory);
            else if (skillID == SkillID.ArmorPiercingBullet)
                skill = new Entity.Equipment.ArmorPiercingBulletSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs, _modifyAttributeEffectFactory);
            else if (skillID == SkillID.TacticalManual)
                skill = new Entity.Equipment.TacticalManualSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs, _modifyAttributeEffectFactory);
            else
                skill = new ActiveSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs);

            skill.SetLevel(1);
            return skill;
        }
    }
}
