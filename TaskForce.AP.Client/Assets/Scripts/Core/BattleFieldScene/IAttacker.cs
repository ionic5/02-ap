using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IAttacker
    {
        Core.Entity.ISkill GetSkill(string skillID);
        Vector2 GetDirection();
        Vector2 GetPosition();
        bool IsEnemy(ITarget entry);
        bool IsTargetInAttackRange(ITarget mainTarget);
    }
}
