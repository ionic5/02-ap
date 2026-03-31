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

            if (skillID == SkillID.MeleeAttack)
                return new Entity.MeleeAttackSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs);

            if (skillID == SkillID.CleavingAttack)
            {
                var effects = _gameDataStore.GetModifyAttributeSkillEffects(skillID);
                var skill = new Entity.ModifyAttributeSkill(skillData, _textStore, baseAttrs, lvAttrs,
                    effects, _modifyAttributeEffectFactory.Create, skillDescs);
                skill.SetLevel(1);
                return skill;
            }

            return new ActiveSkill(skillData, _textStore, baseAttrs, lvAttrs, skillDescs);
        }
    }
}
