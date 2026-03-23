using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.Entity
{
    public class ActiveSkill : Skill
    {
        public ActiveSkill(string skillID, TextStore textStore, IEnumerable<GameData.BaseAttribute> baseAttributes, IEnumerable<GameData.LevelAttribute> levelAttributes) : base(skillID, textStore, baseAttributes, levelAttributes)
        {
        }

        public override void AddToOwner()
        {
            GetOwner().AddSkill(this);
        }
    }
}
