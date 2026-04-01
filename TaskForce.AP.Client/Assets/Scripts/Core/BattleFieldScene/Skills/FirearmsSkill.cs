using System.Collections.Generic;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 모든 총기류 스킬의 공통 로직을 정의하는 추상 클래스입니다.
    /// ActiveSkill을 상속받아 기본 스킬 로직을 공유합니다.
    /// </summary>
    public abstract class FirearmsSkill : ActiveSkill
    {
        protected FirearmsSkill(Core.Entity.ISkill skillEntity) : base(skillEntity)
        {
        }

        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var range = GetAttribute(AttributeID.AttackRange).AsFloat();
            // 거리 계산 로직 필요 (예시)
            return true; 
        }

        public override bool IsCooldownFinished()
        {
            // 쿨타임 체크 로직 필요
            return true;
        }

        public override bool IsCompleted() => true;

        public override void Cancel()
        {
            // 스킬 취소 로직
        }
    }
}