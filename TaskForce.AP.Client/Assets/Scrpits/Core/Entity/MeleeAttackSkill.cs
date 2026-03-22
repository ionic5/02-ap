using System.Collections.Generic;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.Entity
{
    public class MeleeAttackSkill : ActiveSkill, IActiveSkill
    {
        public MeleeAttackSkill(string skillID, TextStore textStore,
            IEnumerable<BaseAttribute> baseAttributes,
            IEnumerable<LevelAttribute> levelAttributes)
            : base(skillID, textStore, baseAttributes, levelAttributes)
        {
        }

        public override Attribute GetAttribute(string attributeID)
        {
            return GetUserAttribute(attributeID);
        }
    }
}
