using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    public class ModifyAttributeSkill : Skill
    {
        private readonly List<IModifyAttributeEffect> _effects;
        private readonly IEnumerable<GameData.ModifyAttributeSkill> _effectDatas;
        private readonly Func<string, int, IModifyAttributeEffect> _createEffect;

        public ModifyAttributeSkill(string skillID, TextStore textStore,
            IEnumerable<GameData.BaseAttribute> baseAttributes,
            IEnumerable<GameData.LevelAttribute> levelAttributes,
            IEnumerable<GameData.ModifyAttributeSkill> effectDatas,
            Func<string, int, IModifyAttributeEffect> createEffect,
            IEnumerable<GameData.SkillDescription> skillDescriptions)
            : base(skillID, textStore, baseAttributes, levelAttributes, skillDescriptions)
        {
            _effectDatas = effectDatas;
            _createEffect = createEffect;
            _effects = new List<IModifyAttributeEffect>();
        }

        public override void SetLevel(int value)
        {
            base.SetLevel(value);

            var owner = GetOwner();
            if (owner != null)
            {
                owner.RemoveModifyAttributeEffects(_effects);
                RecreateEffects();
                owner.AddModifyAttributeEffects(_effects);
            }
            else
            {
                RecreateEffects();
            }
        }

        private void RecreateEffects()
        {
            _effects.Clear();

            foreach (var entry in _effectDatas)
            {
                var effectID = entry.ModifyAttributeEffectID;
                var newEff = _createEffect.Invoke(effectID, 1);
                _effects.Add(newEff);
            }
        }

        public override void OnAddedToUnit(Unit unit)
        {
            base.OnAddedToUnit(unit);
            unit.AddModifyAttributeEffects(_effects);
        }
    }
}
