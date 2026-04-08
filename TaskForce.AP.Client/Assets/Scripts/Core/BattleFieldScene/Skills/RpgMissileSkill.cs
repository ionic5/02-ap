using System;
using System.Collections.Generic;
using System.Numerics;
using System.Xml.Schema;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class RpgMissileSkill : InstantSkill, ISkill
    {
        private readonly Core.Random _random;
        private readonly RepeatTimer _repeatTimer;
        private readonly Core.Timer _timer;
        private readonly Func<IUnit, int, int, float, RpgMissile> _createRpg;
        private ILogger _logger;

        public RpgMissileSkill(Core.Random random, RepeatTimer repeatTimer, Core.Timer timer,
            Func<IUnit, int, int, float, RpgMissile> createRpg, Core.Entity.ISkill skillEntity, ILogger logger) : base(skillEntity)
        {
            _random = random;
            _repeatTimer = repeatTimer;
            _timer = timer;
            _createRpg = createRpg;
            _logger = logger;
        }

        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning();
        }

        public override void Use(UseSkillArgs args)
        {_logger.Info("rpg 사용 시도");
            _timer.Start(GetAttribute(AttributeID.CooldownTime).AsFloat());

            _repeatTimer.Start(() => ThrowRpg(GetOwner()),
                GetAttribute(AttributeID.BurstInterval).AsFloat(),
                GetAttribute(AttributeID.BurstCount).AsInt());
            var coolTime = GetAttribute(AttributeID.CooldownTime).AsFloat();
            var burstInterval = GetAttribute(AttributeID.BurstInterval).AsFloat();
            var burstCount = GetAttribute(AttributeID.BurstCount).AsInt();
            _logger.Info($"rpg 사용 시도: cooldownTime: {coolTime}, burstInterval: {burstInterval}, count: {burstCount}");
        }

        public void ThrowRpg(IUnit user)
        {_logger.Info("rpg: 던짐");
            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();
            var explosionRadius = GetAttribute(AttributeID.ExplosionRadius).AsFloat();
             var missile = _createRpg.Invoke(user, minDmg, maxDmg, explosionRadius);

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

            return distSq >= minRange * minRange && distSq <= maxRange * maxRange;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            var minRange = GetAttribute(AttributeID.MinMissileRange).AsFloat();
            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();

            return unit.FindTargets(minRange, maxRange);
        }
    }
}
