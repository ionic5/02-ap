using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 지뢰 설치 스킬을 담당합니다.
    /// </summary>
    public class LandmineSkill : InstantSkill, ISkill
    {
        private readonly Core.Timer _timer;
        private readonly Func<IUnit, int, int, float, float, float, Landmine> _createLandmine;

        public LandmineSkill(Core.Entity.ISkill skillEntity,
            Core.Timer timer, Func<IUnit, int, int, float, float, float, Landmine> createLandmine) : base(skillEntity)
        {
            _timer = timer;
            _createLandmine = createLandmine;
        }

        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning();
        }

        public override void Use(UseSkillArgs args)
        {
            var user = GetOwner();

            _timer.Start(GetAttribute(AttributeID.CooldownTime).AsFloat());

            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();
            var watchRadius = GetAttribute(AttributeID.WatchRadius).AsFloat();
            var explosionRadius = GetAttribute(AttributeID.ExplosionRadius).AsFloat();
            var expireTime = GetAttribute(AttributeID.ExpireTime).AsFloat();

            var landmine = _createLandmine(user, minDmg, maxDmg, watchRadius, explosionRadius, expireTime);
            landmine.Plant(user.GetPosition());
        }

        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var range = GetAttribute(AttributeID.ExplosionRadius).AsFloat();
            var distSq = Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition());

            return distSq <= range * range;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return unit.FindTargets(GetAttribute(AttributeID.ExplosionRadius).AsFloat());
        }
    }
}
