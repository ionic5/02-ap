using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 대물 저격총 스킬 로직을 담당합니다. 강력한 단발 관통 탄환을 발사합니다.
    /// </summary>
    public class SniperSkill : InstantSkill, ISkill
    {
        private readonly Func<IUnit, int, ITargetFinder, Bullet> _createBullet;
        private readonly ITargetFinder _targetFinder;
        private readonly Core.Timer _timer;

        public SniperSkill(Func<IUnit, int, ITargetFinder, Bullet> createBullet, ITargetFinder targetFinder, 
            Core.Timer timer, Core.Entity.ISkill skillEntity)
            : base(skillEntity)
        {
            _createBullet = createBullet;
            _targetFinder = targetFinder;
            _timer = timer;
        }

        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning();
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            var range = GetAttribute(AttributeID.MissileRange).AsFloat();
            if (range <= 0) range = 20f; // 기본 사거리Fallback
            return unit.FindTargets(range);
        }

        public override void Use(UseSkillArgs args)
        {
            if (!IsCooldownFinished())
            {
                return;
            }

            var owner = GetOwner();
            if (owner == null || owner.IsDead())
            {
                return;
            }

            // 자동 발사를 위해 타겟이 없으면 가장 가까운 타겟을 찾음
            ITarget target = args.Target;
            if (target == null)
            {
                var targets = GetTargetsInRange(owner);
                if (targets == null || !targets.Any())
                {
                    // 주변에 적이 없으면 아예 발사하지 않음 (사운드 및 탄환 생성 방지)
                    return;
                }
                target = targets.OrderBy(t => Vector2.DistanceSquared(owner.GetPosition(), t.GetPosition())).First();
            }

            if (target == null || !target.IsAlive())
            {
                return;
            }

            // 적이 확실히 있을 때만 쿨타임 시작
            var cooldown = GetAttribute(AttributeID.CooldownTime).AsFloat();
            if (cooldown <= 0) cooldown = 1.0f;
            _timer.Start(cooldown);

            // 속성 정보 가져오기
            var damage = GetAttribute(AttributeID.Damage).AsInt();
            if (damage <= 0) damage = GetAttribute(AttributeID.MinDamage).AsInt();
            if (damage <= 0) damage = 50;

            var missileSpeed = GetAttribute(AttributeID.MissileSpeed).AsFloat();
            if (missileSpeed <= 0) missileSpeed = 20f;

            // 저격총 탄환 생성 및 발사
            var bullet = _createBullet.Invoke(owner, damage, _targetFinder);
            if (bullet != null)
            {
                bullet.SetPosition(owner.GetPosition());
                bullet.SetTarget(target);
                bullet.SetSpeed(missileSpeed);
                bullet.Start();
            }

            args.OnCompleted?.Invoke();
        }

        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var range = GetAttribute(AttributeID.MissileRange).AsFloat();
            if (range <= 0) range = 20f;
            return Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition()) <= range * range;
        }
    }
}
