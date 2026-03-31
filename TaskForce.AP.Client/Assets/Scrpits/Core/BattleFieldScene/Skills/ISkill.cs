using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public interface ISkill
    {
        bool IsInstantSkill();
        bool HasHealEffect();
        bool IsTargetInRange(IUnit unit, ITarget target);
        IEnumerable<ITarget> GetTargetsInRange(IUnit unit);
        string GetSkillID();
        void OnAddedToUnit(IUnit owner);
        bool IsCooldownFinished();
        void Use(UseSkillArgs args);
        bool IsCompleted();
        void Cancel();
    }
}
