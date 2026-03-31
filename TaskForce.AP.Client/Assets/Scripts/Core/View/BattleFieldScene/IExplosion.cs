using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IExplosion : IDestroyable
    {
        event EventHandler<BatchObjectHitEventArgs> BatchObjectHitEvent;

        event EventHandler ExplosionFinishedEvent;

        void Start(float explosionRadius);
        void SetPosition(Vector2 vector2);
    }
}
