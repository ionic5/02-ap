using System;
using System.Collections.Generic;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 권총 스킬 로직을 담당합니다. 총알을 발사하는 기능을 수행합니다.
    /// </summary>
    public class PistolSkill : FirearmsSkill
    {
        private readonly Func<IUnit, int, ITargetFinder, Bullet> _createBullet;
        private readonly ITargetFinder _targetFinder;

        public PistolSkill(Func<IUnit, int, ITargetFinder, Bullet> createBullet, ITargetFinder targetFinder, Core.Entity.ISkill skillEntity)
            : base(skillEntity)
        {
            _createBullet = createBullet;
            _targetFinder = targetFinder;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            // 실제 구현 필요 (범위 내 타겟 검색)
            return new List<ITarget>();
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

            // 스킬 데이터에서 속성 정보 가져오기 (AttributeID.Damage 사용)
            var damage = GetAttribute(AttributeID.Damage).AsInt();
            var missileSpeed = GetAttribute(AttributeID.MissileSpeed).AsFloat();

            // 총알 생성 및 발사 (단일 데미지 전달)
            var bullet = _createBullet.Invoke(owner, damage, _targetFinder);
            bullet.SetPosition(owner.GetPosition());
            bullet.SetTarget(args.Target);
            bullet.SetSpeed(missileSpeed);
            bullet.Start();

            args.OnCompleted?.Invoke();
        }
    }
}