using System;
using TaskForce.AP.Client.Core;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class PoolableObject : Object, IDestroyable
    {
        private Action<GameObject> _returnToPoolHanlder;

        public virtual void Revive()
        {
            _isDestroyed = false;
        }

        public void SetReturnToPoolHandler(Action<GameObject> returnToPoolHandler)
        {
            _returnToPoolHanlder = returnToPoolHandler;
        }

        protected override void HandleDestroy()
        {
            var hdlr = _returnToPoolHanlder;
            OnDestroy();
            hdlr.Invoke(gameObject);
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            _returnToPoolHanlder = null;
        }
    }
}
