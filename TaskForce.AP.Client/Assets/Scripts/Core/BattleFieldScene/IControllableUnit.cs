using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IControllableUnit : IUnit, IFollowable
    {
        IUnit GetMaster();
        float GetFollowRange();
        void AddExp(int exp);
        float GetPickUpRange();
        bool IsTargetInAttackRange(ITarget mainTarget);
        void Move(Vector2 direction);
        void MoveTo(Vector2 vector2);
        IEnumerable<ISkill> GetSkills();
    }
}
