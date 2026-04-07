using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 별도의 사용 로직이 없는 패시브 스킬(장비)을 위한 클래스입니다.
    /// </summary>
    public class PassiveSkill : ISkill
    {
        private readonly Entity.ISkill _skillEntity;

        public PassiveSkill(Entity.ISkill skillEntity)
        {
            _skillEntity = skillEntity;
        }

        public string GetSkillID() => _skillEntity.GetSkillID();

        public void OnAddedToUnit(IUnit owner) { }

        public bool IsInstantSkill() => false;

        public bool HasHealEffect() => false;

        public bool IsTargetInRange(IUnit unit, ITarget target) => false;

        public IEnumerable<ITarget> GetTargetsInRange(IUnit unit) => Enumerable.Empty<ITarget>();

        public bool IsCooldownFinished() => true;

        public void Use(UseSkillArgs args) { }

        public bool IsCompleted() => true;

        public void Cancel() { }
    }
}
