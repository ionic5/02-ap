using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class SkillFactory
    {
        private readonly Dictionary<string, Func<Entity.ISkill, ISkill>> _creators;

        public SkillFactory()
        {
            _creators = new Dictionary<string, Func<Core.Entity.ISkill, ISkill>>();
        }

        public void AddCreator(string key, Func<Entity.ISkill, ISkill> tmp)
        {
            _creators.Add(key, tmp);
        }

        public ISkill Create(Entity.ISkill skill)
        {
            if (_creators.TryGetValue(skill.GetSkillID(), out Func<Entity.ISkill, ISkill> func))
                return func.Invoke(skill);
            return null;
        }
    }
}
