using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class HealSkill : ActiveSkill, ISkill
    {
        private readonly Core.Timer _timer;
        private readonly Func<IOneShotEffect> _createHealEffect;

        private struct TimerType
        {
            public const int Cooldown = 0;
            public const int ApplyDelay = 1;
            public const int Casting = 2;
        }

        public HealSkill(Timer timer, Func<IOneShotEffect> createHealEffect, Core.Entity.ISkill skill) : base(skill)
        {
            _timer = timer;
            _createHealEffect = createHealEffect;
        }

        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning(TimerType.Cooldown);
        }

        public override void Use(UseSkillArgs args)
        {
            var user = GetOwner();
            user.Cast(GetAttribute(AttributeID.CastTime).AsFloat());
            _timer.Start(TimerType.Casting, GetAttribute(AttributeID.CastTime).AsFloat(), () =>
            {
                var target = args.Target;
                if (target.IsDead())
                    return;

                _timer.Start(TimerType.Cooldown, GetAttribute(AttributeID.CooldownTime).AsFloat());

                var effect = _createHealEffect.Invoke();
                effect.SetPosition(target.GetPosition());
                effect.Follow(target);
                effect.Play();

                _timer.Start(TimerType.ApplyDelay, GetAttribute(AttributeID.ApplyDelayTime).AsFloat(), () =>
                {
                    target.Heal(GetAttribute(AttributeID.HealAmount).AsInt());
                });
            });
        }

        public override bool HasHealEffect()
        {
            return true;
        }

        public override bool IsTargetInRange(IUnit unit, ITarget mainTarget)
        {
            var range = GetAttribute(AttributeID.Range).AsFloat();
            return Vector2.DistanceSquared(unit.GetPosition(), mainTarget.GetPosition()) < range * range;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return unit.FindAllies(GetAttribute(AttributeID.CastRange).AsFloat());
        }

        public override bool IsCompleted()
        {
            return !_timer.IsRunning(TimerType.Casting);
        }

        public override void Cancel()
        {
            if (_timer.IsRunning(TimerType.Casting))
                _timer.Stop(TimerType.Casting);
        }
    }
}
