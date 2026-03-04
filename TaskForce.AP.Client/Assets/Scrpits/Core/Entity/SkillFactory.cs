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

        public ISkill CreateSkillEntity(Entity.Unit owner, string skillID, int level)
        {
            var skill = CreateSkill(skillID);
            skill.SetOwner(owner);
            skill.SetLevel(level);

            return skill;
        }

        public ISkill CreateSkill(string skillID)
        {
            var skillData = _gameDataStore.GetSkills().Where(entry => entry.ID == skillID).FirstOrDefault();
            if (skillData == null)
            {
                _logger.Fatal($"Failed to find skill data for {skillID}");
                return null;
            }

            if (skillID == SkillID.MeleeAttack)
                return new Entity.MeleeAttackSkill(skillID, skillData, _textStore);

            if (skillID == SkillID.CleavingAttack)
            {
                var effects = _gameDataStore.GetModifyAttributeSkillEffects(skillID);
                var skill = new Entity.ModifyAttributeSkill(skillID, skillData,
                    _textStore, effects, _modifyAttributeEffectFactory.Create);
                skill.SetLevel(1);
                return skill;
            }

            var attributeMediator = new AttributeMediator(_gameDataStore, _logger);

            attributeMediator.SetBaseAttributes(_gameDataStore.GetSkillBaseAttributes(skillID));
            attributeMediator.SetGrowthFormulas(_gameDataStore.GetSkillGrowthFormulas(skillID));

            return new Entity.CommonSkill(skillID, skillData, _textStore, attributeMediator);
        }
    }
}
