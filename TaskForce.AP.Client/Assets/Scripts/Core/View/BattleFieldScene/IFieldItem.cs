using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IFieldItem : IDestroyable
    {
        event EventHandler SpawnCompletedEvent;
        Vector2 GetPosition();
        void SetPosition(Vector2 position);
    }
}
