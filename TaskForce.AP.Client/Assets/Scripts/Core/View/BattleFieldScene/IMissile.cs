using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IMissile : IDestroyable
    {
        event EventHandler ArrivedDestinationEvent;
        event EventHandler<View.HitEventArgs> HitEvent;

        void MoveTo(Vector2 destination, float speed);
        void SetPosition(Vector2 position);
        Vector2 GetPosition();
    }
}
