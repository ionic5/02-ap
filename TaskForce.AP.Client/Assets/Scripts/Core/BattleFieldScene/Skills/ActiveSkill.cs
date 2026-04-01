using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public abstract class ActiveSkill : ISkill
    {
        private readonly Entity.ISkill _skillEntity;
        private IUnit _owner;

        public ActiveSkill(Core.Entity.ISkill skillEntity)
        {
            _skillEntity = skillEntity;
        }

        public string GetSkillID()
        {
            return _skillEntity.GetSkillID();
        }

        public void OnAddedToUnit(IUnit owner)
        {
            _owner = owner;
        }

        protected IUnit GetOwner()
        {
            return _owner;
        }

        protected Variant GetAttribute(string attributeID)
        {
            return _skillEntity.GetAttribute(attributeID);
        }

        public virtual bool IsInstantSkill()
        {
            return false;
        }

        public virtual bool HasHealEffect()
        {
            return false;
        }

        public abstract bool IsTargetInRange(IUnit unit, ITarget target);
        public abstract IEnumerable<ITarget> GetTargetsInRange(IUnit unit);

        public abstract bool IsCompleted();
        public abstract bool IsCooldownFinished();
        public abstract void Use(UseSkillArgs args);
        public abstract void Cancel();
    }
}
