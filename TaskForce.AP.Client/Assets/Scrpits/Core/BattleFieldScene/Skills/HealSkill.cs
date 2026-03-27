using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class HealSkill : ActiveSkill, ISkill
    {
        private readonly Core.Timer _cooldownTimer;
        private readonly Core.Timer _applyDelayTimer;
        private readonly Core.Timer _castingTimer;
        private readonly Func<IOneShotEffect> _createHealEffect;

        public HealSkill(Func<Timer> createTimer, Func<IOneShotEffect> createHealEffect, Core.Entity.ISkill skill) : base(skill)
        {
            _cooldownTimer = createTimer();
            _applyDelayTimer = createTimer();
            _castingTimer = createTimer();
            _createHealEffect = createHealEffect;
        }

        public override bool IsCooldownFinished()
        {
            return !_cooldownTimer.IsRunning();
        }

        public override void Use(UseSkillArgs args)
        {
            var user = GetOwner();
            user.Cast(GetAttribute(AttributeID.CastTime).AsFloat());
            _castingTimer.Start(GetAttribute(AttributeID.CastTime).AsFloat(), () =>
            {
                var target = args.Target;
                if (target.IsDead())
                    return;

                _cooldownTimer.Start(GetAttribute(AttributeID.CooldownTime).AsFloat());

                var effect = _createHealEffect.Invoke();
                effect.SetPosition(target.GetPosition());
                effect.Follow(target);
                effect.Play();

                _applyDelayTimer.Start(GetAttribute(AttributeID.ApplyDelayTime).AsFloat(), () =>
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
            return !_castingTimer.IsRunning();
        }

        public override void Cancel()
        {
            if (_castingTimer.IsRunning())
                _castingTimer.Stop();
        }
    }
}
