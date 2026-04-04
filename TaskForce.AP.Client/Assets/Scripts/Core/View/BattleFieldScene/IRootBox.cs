using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IRootBox : IDestroyable
    {
        Vector2 GetPosition();
        void SetPosition(Vector2 position);
        string GetObjectID();
    }
}
