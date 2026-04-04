using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IFieldItem : IDestroyable
    {
        Vector2 GetPosition();
        void SetPosition(Vector2 position);
    }
}
