using System.Collections.Generic;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.Entity.Equipment
{
    public class GlovesSkill : Skill
    {
        private readonly ModifyAttributeEffectFactory _effectFactory;
        private IModifyAttributeEffect _currentEffect;

        public GlovesSkill(GameData.Skill skillData, TextStore textStore,
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

            // 기존 효과가 있다면 제거
            if (_currentEffect != null)
            {
                owner.RemoveModifyAttributeEffects(new[] { _currentEffect });
            }

            // 새로운 레벨에 맞는 효과 생성 및 등록
            // "GLOVES_COOLDOWN_REDUCTION" 효과 ID를 사용한다고 가정
            _currentEffect = _effectFactory.Create("GLOVES_COOLDOWN_REDUCTION", GetLevel());
            if (_currentEffect != null)
            {
                owner.AddModifyAttributeEffect(_currentEffect);
            }
        }
    }
}
