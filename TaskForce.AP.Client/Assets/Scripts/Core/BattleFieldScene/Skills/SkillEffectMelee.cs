using System;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class SkillEffectMelee : IUpdatable
    {
        private readonly ISkillEffect _skillEffectDagger;
        private ILoop _loop;
        private IUnit _unit;

        public SkillEffectMelee(ISkillEffect skillEffectDagger, ILoop loop)
        {
            _skillEffectDagger = skillEffectDagger;
            _loop = loop;

            _skillEffectDagger.DestroyEvent += OnDestroyEvent;
            
            _loop.Add(this);
        }
        
        public void Update()
        {
            if (_unit == null)
                return;
            
            _skillEffectDagger.SetPosition(_unit.GetPosition());
        }

        private void OnDestroyEvent(object sender, EventArgs e)
        {
            _loop.Remove(this);
            _unit = null;
        }

        public void SetRotation(Vector2 rotation)
        {
            if (_unit == null)
                return;
            
            _skillEffectDagger.SetRotation(rotation);
        }

        public void SetFollow(IUnit unit)
        {
            _unit = unit;
        }
    }
}
