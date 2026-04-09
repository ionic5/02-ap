using System;
using System.Collections.Generic;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 대물 저격총 스킬 로직을 담당합니다. 강력한 단발 관통 탄환을 발사합니다.
    /// </summary>
    public class SniperSkill : FirearmsSkill
    {
        private readonly Func<IUnit, int, ITargetFinder, Bullet> _createBullet;
        private readonly ITargetFinder _targetFinder;

        public SniperSkill(Func<IUnit, int, ITargetFinder, Bullet> createBullet, ITargetFinder targetFinder, Core.Entity.ISkill skillEntity)
            : base(skillEntity)
        {
            _createBullet = createBullet;
            _targetFinder = targetFinder;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            // 실제 구현 필요 (범위 내 타겟 검색 - 긴 사거리 반영)
            var range = GetAttribute(AttributeID.MissileRange).AsFloat();
            return unit.FindTargets(range);
        }

        public override void Use(UseSkillArgs args)
        {
            var owner = GetOwner();
            if (owner == null || owner.IsDead() || args.Target == null)
            {
                return;
            }

            // 타겟이 살아있는지 확인
            if (!args.Target.IsAlive())
            {
                return;
            }

            // 단일 데미지(DAMAGE) 및 탄속 속성 가져오기
            var damage = GetAttribute(AttributeID.Damage).AsInt();
            var missileSpeed = GetAttribute(AttributeID.MissileSpeed).AsFloat();

            // 저격총 탄환 생성 및 발사
            var bullet = _createBullet.Invoke(owner, damage, _targetFinder);
            bullet.SetPosition(owner.GetPosition());
            bullet.SetTarget(args.Target);
            bullet.SetSpeed(missileSpeed);
            bullet.Start();

            args.OnCompleted?.Invoke();
        }
    }
}
