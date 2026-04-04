using System.Collections.Generic;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.Entity
{
    public class MeleeDaggerSkill : ActiveSkill, ISkill
    {
        public MeleeDaggerSkill(GameData.Skill skillData, TextStore textStore,
            IEnumerable<BaseAttribute> baseAttributes,
            IEnumerable<LevelAttribute> levelAttributes,
            IEnumerable<SkillDescription> skillDescriptions)
            : base(skillData, textStore, baseAttributes, levelAttributes, skillDescriptions)
        {
        }

        // TODO: JW: 추후 삭제 확인 요
        // public override Variant GetAttribute(string attributeID)
        // {
        //     // return base.GetAttribute(attributeID);
        //     return GetOwner().GetAttribute(attributeID);
        // }
    }
}