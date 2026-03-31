using System.Collections.Generic;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.Entity
{
    public class MeleeAttackSkill : ActiveSkill, ISkill
    {
        public MeleeAttackSkill(GameData.Skill skillData, TextStore textStore,
            IEnumerable<BaseAttribute> baseAttributes,
            IEnumerable<LevelAttribute> levelAttributes,
            IEnumerable<SkillDescription> skillDescriptions)
            : base(skillData, textStore, baseAttributes, levelAttributes, skillDescriptions)
        {
        }

        public override Attribute GetAttribute(string attributeID)
        {
            return GetOwner().GetAttribute(attributeID);
        }
    }
}
