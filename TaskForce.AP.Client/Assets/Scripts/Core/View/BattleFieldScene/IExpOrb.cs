using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IExpOrb : IDestroyable
    {
        Vector2 GetPosition();
        void MoveTo(Core.BattleFieldScene.IFollowable followTarget, float speed);
        void SetPosition(Vector2 position);
        void Stop();
    }
}
