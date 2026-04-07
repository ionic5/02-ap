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
    }
}