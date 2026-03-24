using System;
using UnityEngine;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;

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
            ClosedEvent?.Invoke(this, EventArgs.Empty);

            Clear();
        }

        public virtual void Clear()
        {
            ClosedEvent = null;
        }
    }
}