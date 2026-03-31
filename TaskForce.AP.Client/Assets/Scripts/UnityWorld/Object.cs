using System;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class Object : MonoBehaviour, IDestroyable
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        protected bool _isDestroyed;

        public void Destroy()
        {
            if (_isDestroyed)
                return;

            HandleDestroy();
        }

        protected virtual void HandleDestroy()
        {
            Destroy(gameObject);
        }

        protected void OnDestroy()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));

            CleanUp();
        }

        protected virtual void CleanUp()
        {
            DestroyEvent = null;
        }
    }
}
