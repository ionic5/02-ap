using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class MonkSkill : InstantSkill, ISkill
    {
        private int _summonCount;
        private readonly Func<string, int, IUnit> _createUnit;

        public MonkSkill(Entity.ISkill skillEntity, Func<string, int, IUnit> createUnit) : base(skillEntity)
        {
            _createUnit = createUnit;
            _summonCount = 0;
        }

        public override bool IsCooldownFinished()
        {
            return _summonCount <= 0;
        }

        public override void Use(UseSkillArgs args)
        {
            var summoner = GetOwner();

            _summonCount++;

            var unit = _createUnit("MONK", 1);
            unit.SetPosition(summoner.GetPosition());
            unit.SetMaster(summoner);
        }

        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            return false;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return Enumerable.Empty<ITarget>();
        }
    }
}
