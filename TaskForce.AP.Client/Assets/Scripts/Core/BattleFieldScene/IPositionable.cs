using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IPositionable
    {
        void SetPosition(Vector2 vector2);
        Vector2 GetPosition();
    }
}
