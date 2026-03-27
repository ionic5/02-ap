using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class DynamiteSkill : InstantSkill, ISkill
    {
        private readonly Core.Random _random;
        private readonly RepeatTimer _repeatTimer;
        private readonly Core.Timer _timer;
        private readonly Func<IUnit, int, int, float, Dynamite> _createDynamite;

        public DynamiteSkill(Random random, RepeatTimer repeatTimer, Timer timer,
            Func<IUnit, int, int, float, Dynamite> createDynamite, Core.Entity.ISkill skillEntity) : base(skillEntity)
        {
            _random = random;
            _repeatTimer = repeatTimer;
            _timer = timer;
            _createDynamite = createDynamite;
        }

        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning();
        }

        public override void Use(UseSkillArgs args)
        {
            _timer.Start(GetAttribute(AttributeID.CooldownTime).AsFloat());

            _repeatTimer.Start(() => ThrowDynamite(GetOwner()),
                GetAttribute(AttributeID.BurstInterval).AsFloat(),
                GetAttribute(AttributeID.BurstCount).AsInt());
        }

        public void ThrowDynamite(IUnit user)
        {
            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();
            var explosionRadius = GetAttribute(AttributeID.ExplosionRadius).AsFloat();
            var missile = _createDynamite.Invoke(user, minDmg, maxDmg, explosionRadius);

            var casterPos = user.GetPosition();
            missile.SetPosition(casterPos);

            var minRange = GetAttribute(AttributeID.MinMissileRange).AsFloat();
            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();
            var missleSpd = GetAttribute(AttributeID.MissileSpeed).AsFloat();
            var targetPos = _random.NextPosition(casterPos, minRange, maxRange);
            missile.MoveTo(targetPos, missleSpd);
        }

        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var minRange = GetAttribute(AttributeID.MinMissileRange).AsFloat();
            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();
            var distSq = Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition());

            return distSq >= minRange * minRange && distSq >= maxRange * maxRange;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            var minRange = GetAttribute(AttributeID.MinMissileRange).AsFloat();
            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();

            return unit.FindTargets(minRange, maxRange);
        }
    }
}
