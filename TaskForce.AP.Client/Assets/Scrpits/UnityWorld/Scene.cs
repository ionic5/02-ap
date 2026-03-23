using System;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class Scene : MonoBehaviour, IDestroyable
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        public void Destroy()
        {
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;
        }
    }
}
