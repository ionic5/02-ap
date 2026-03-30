using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    public class ActiveSkill : Skill
    {
        public ActiveSkill(GameData.Skill skillData, TextStore textStore, 
            IEnumerable<GameData.BaseAttribute> baseAttributes, IEnumerable<GameData.LevelAttribute> levelAttributes, 
            IEnumerable<GameData.SkillDescription> skillDescriptions) 
            : base(skillData, textStore, baseAttributes, levelAttributes, skillDescriptions)
        {
        }

    }
}
