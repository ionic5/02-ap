using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class SheepMissileSkill : InstantSkill, ISkill
    {
        private readonly Core.Random _random;
        private readonly RepeatTimer _repeatTimer;
        private readonly Core.Timer _timer;
        private readonly Func<int, int, SheepMissile> _createSheepMissile;

        public SheepMissileSkill(Random random, RepeatTimer repeatTimer, Timer timer,
            Func<int, int, SheepMissile> createSheepMissile, Core.Entity.ISkill skillEntity) : base(skillEntity)
        {
            _random = random;
            _repeatTimer = repeatTimer;
            _timer = timer;
            _createSheepMissile = createSheepMissile;
        }

        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning(0);
        }

        public override void Use(UseSkillArgs args)
        {
            var user = GetOwner();

            var maxRange = GetAttribute(AttributeID.MaxMissileRange).AsFloat();
            var targets = user.FindTargets(maxRange);

            if (!targets.Any())
                return;

            var target = targets.ElementAt(_random.Next(0, targets.Count()));
            _timer.Start(0, GetAttribute(AttributeID.CooldownTime).AsFloat());

            _repeatTimer.Start(() => { FireMissile(user, target.GetPosition()); },
                GetAttribute(AttributeID.BurstInterval).AsFloat(),
                GetAttribute(AttributeID.BurstCount).AsInt());
        }

        protected void FireMissile(IUnit user, Vector2 targetPos)
        {
            var shooterPos = user.GetPosition();
            var direction = Vector2.Normalize(targetPos - shooterPos);

            var launchAngle = GetAttribute(AttributeID.LaunchAngle).AsFloat();
            float deltaAngleDeg = (float)(_random.NextDouble() * 2.0f * launchAngle - launchAngle);

            float deltaAngleRad = deltaAngleDeg * MathF.PI / 180f;

            float cos = MathF.Cos(deltaAngleRad);
            float sin = MathF.Sin(deltaAngleRad);
            var rotatedDir = new Vector2(
                direction.X * cos - direction.Y * sin,
                direction.X * sin + direction.Y * cos
            );

            var range = GetAttribute(AttributeID.MissileRange).AsFloat();
            var destination = shooterPos + rotatedDir * range;

            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var missile = _createSheepMissile.Invoke(minDmg, maxDmg);
            missile.SetPosition(shooterPos);

            var missleSpd = GetAttribute(AttributeID.MissileSpeed).AsFloat();
            missile.MoveTo(destination, missleSpd);
        }

        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var range = GetAttribute(AttributeID.MaxMissileRange).AsFloat();
            var distSq = Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition());
            return distSq <= range * range;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return unit.FindTargets(GetAttribute(AttributeID.MaxMissileRange).AsFloat());
        }
    }
}
