using System;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    public class Window : MonoBehaviour
    {
        public event EventHandler ClosedEvent;

        private void OnDestroy()
        {
            Clear();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            ClosedEvent?.Invoke(this, EventArgs.Empty);

            Clear();
        }

        public virtual void Clear()
        {
            ClosedEvent = null;
        }
    }
}