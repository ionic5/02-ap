using System;

namespace TaskForce.AP.Client.Core
{
    public interface IDestroyable
    {
        event EventHandler<DestroyEventArgs> DestroyEvent;
        void Destroy();
    }
}
