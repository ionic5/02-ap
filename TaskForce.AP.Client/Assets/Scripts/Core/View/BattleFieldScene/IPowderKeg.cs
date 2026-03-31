using System;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IPowderKeg : IPositionable, IDestroyable
    {
        event EventHandler<BatchObjectDetectedEventArgs> BatchObjectDetectedEvent;
        event EventHandler ExplosionEvent;

        void Ignite();
        void Watch(float watchRadius);
    }
}
