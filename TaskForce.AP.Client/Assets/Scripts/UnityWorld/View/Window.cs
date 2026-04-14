using System;
using UnityEngine;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.UnityWorld.View
{
    public class Window : MonoBehaviour, IWindow
    {
        public event EventHandler ClosedEvent;

        private void OnDestroy()
        {
            Clear();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            var handler = ClosedEvent;
            Clear();
            handler?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Clear()
        {
            ClosedEvent = null;
        }
    }
}