using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class World : MonoBehaviour, Core.View.BattleFieldScene.IWorld
    {
        public event EventHandler PausedEvent;
        public event EventHandler ResumedEvent;

        [SerializeField]
        private GameObject _playerUnitSpawnPosition;
        [SerializeField]
        private GameObject _spawnPositionSet;
        [SerializeField]
        private Camera _camera;
        [SerializeField]
        private float _spawnAreaWidth;
        [SerializeField]
        private float _spawnRadius;
        [SerializeField]
        private float _unitMaxRadius;

        public Core.Random Random;

        private List<Vector3> _spawnPositions;
        private bool _isPaused;

        private void Awake()
        {
            _spawnPositions = new List<Vector3>();
            for (int i = 0; i < _spawnPositionSet.transform.childCount; i++)
            {
                var parentTransform = _spawnPositionSet.transform.GetChild(i);
                for (int j = 0; j < parentTransform.childCount; j++)
                    _spawnPositions.Add(parentTransform.GetChild(j).position);
            }
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

        public System.Numerics.Vector2 GetWarpPoint()
        {
            Vector3 pos = GetRandomSpawnPosition();
            return new System.Numerics.Vector2(pos.x, pos.z);
        }

        public bool IsOutOfCameraView(System.Numerics.Vector2 position)
        {
            return IsOutOfCameraView(new Vector3(position.X, 0, position.Y));
        }

        private bool IsOutOfCameraView(Vector3 worldPos)
        {
            Vector3 viewportPos = _camera.WorldToViewportPoint(worldPos);

            return viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1 || viewportPos.z < 0;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            var centerPosList = GetAvailableSpawnPositions().ToList();
            if (centerPosList.Count == 0) return _playerUnitSpawnPosition.transform.position;

            Vector3 centerPos = centerPosList[Random.Next(centerPosList.Count)];

            var randomOffset = Random.NextPosition(System.Numerics.Vector2.Zero, _spawnRadius);
            Vector3 spawnPos = new Vector3(centerPos.x + randomOffset.X, centerPos.y, centerPos.z + randomOffset.Y);

            return spawnPos;
        }

        private IEnumerable<Vector3> GetAvailableSpawnPositions()
        {
            var positions = new List<Vector3>();
            foreach (var pos in _spawnPositions)
                if (IsOutOfCameraView(pos))
                    positions.Add(pos);

            return positions;
        }

        public System.Numerics.Vector2 GetPlayerUnitSpawnPosition()
        {
            var pos = _playerUnitSpawnPosition.transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }

        private void Update()
        {
            // 외부 에서 강제로 pause 를 푸는 상황을 막기 위한 코드
            if (_isPaused && UnityEngine.Time.timeScale != 0.0f)
                UnityEngine.Time.timeScale = 0.0f;
        }
    }
}
