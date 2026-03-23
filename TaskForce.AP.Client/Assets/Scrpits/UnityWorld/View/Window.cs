using System;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene;
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

            // 일시 정지 원복
            if (PauseManager.Instance != null && PauseManager.Instance.IsPaused())
                PauseManager.Instance.TogglePause();
        }

        public virtual void Clear()
        {
            ClosedEvent = null;
        }
    }
}