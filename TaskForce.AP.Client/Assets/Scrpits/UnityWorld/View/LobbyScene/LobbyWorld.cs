using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace TaskForce.AP.Client.UnityWorld.View.LobbyScene
{
    // TODO: JW: IWorld 를 범용으로 사용해도 괜찮을지 결정
    public class LobbyWorld : MonoBehaviour, Core.View.BattleFieldScene.IWorld 
    {
        public event EventHandler PausedEvent;
        public event EventHandler ResumedEvent;

        // TODO: JW: 필요한지 확인 요
        // public Core.Random Random;
        
        private bool _isPaused;

        private void Awake()
        {
        }

        public void Pause()
        {
            if (_isPaused)
                return;

            _isPaused = true;
            UnityEngine.Time.timeScale = 0f;
            AudioListener.pause = true;

            PausedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Resume()
        {
            if (!_isPaused)
                return;

            _isPaused = false;
            UnityEngine.Time.timeScale = 1f;
            AudioListener.pause = false;

            ResumedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public Vector2 GetPlayerUnitSpawnPosition()
        {
            return default;
        }

        public Vector2 GetWarpPoint()
        {
            return default;
        }

        public bool IsOutOfCameraView(Vector2 vector2)
        {
            return false;
        }
    }
}
