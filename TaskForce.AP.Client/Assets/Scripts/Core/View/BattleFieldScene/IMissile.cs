using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IMissile : IDestroyable
    {
        event EventHandler ArrivedDestinationEvent;
        event EventHandler<HitEventArgs> HitEvent;

        void MoveTo(Vector2 destination, float speed);
        void SetTarget(string viewID);
        void SetSpeed(float speed);
        void SetPosition(Vector2 position);
        void Start();
        Vector2 GetPosition();
    }
}
