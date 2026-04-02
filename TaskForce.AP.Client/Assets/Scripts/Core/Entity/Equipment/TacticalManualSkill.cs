using System.Collections.Generic;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.Entity.Equipment
{
    public class TacticalManualSkill : Skill
    {
        private readonly ModifyAttributeEffectFactory _effectFactory;
        private IModifyAttributeEffect _currentEffect;

        public TacticalManualSkill(GameData.Skill skillData, TextStore textStore,
            IEnumerable<BaseAttribute> baseAttributes, IEnumerable<LevelAttribute> levelAttributes,
            IEnumerable<SkillDescription> skillDescriptions, ModifyAttributeEffectFactory effectFactory)
            : base(skillData, textStore, baseAttributes, levelAttributes, skillDescriptions)
        {
            _effectFactory = effectFactory;
        }

        public override void OnAddedToUnit(Unit unit)
        {
            base.OnAddedToUnit(unit);
            ApplyEffect();
        }

        public override void SetLevel(int value)
        {
            base.SetLevel(value);
            ApplyEffect();
        }

        private void ApplyEffect()
        {
            var owner = GetOwner();
            if (owner == null) return;

            if (_currentEffect != null)
            {
                owner.RemoveModifyAttributeEffects(new[] { _currentEffect });
            }

            // "TACTICAL_MANUAL" 효과 ID를 사용
            _currentEffect = _effectFactory.Create("TACTICAL_MANUAL", GetLevel());
            if (_currentEffect != null)
            {
                owner.AddModifyAttributeEffect(_currentEffect);
            }
        }
    }
}
