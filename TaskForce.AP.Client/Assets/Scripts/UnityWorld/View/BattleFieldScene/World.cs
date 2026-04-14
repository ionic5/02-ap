using System;
using UnityEngine;
using UnityEngine.AI;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class World : MonoBehaviour, Core.View.BattleFieldScene.IWorld
    {
        public event EventHandler PausedEvent;
        public event EventHandler ResumedEvent;

        [SerializeField]
        private GameObject _playerUnitSpawnPosition;
        [SerializeField]
        private Camera _camera;

        public Core.Random Random;

        private Core.BattleFieldScene.IUnit _player;
        private float _spawnMinRadius;
        private float _spawnMaxRadius;
        private bool _isPaused;

        public void SetPlayer(Core.BattleFieldScene.IUnit player)
        {
            _player = player;
        }

        public void SetSpawnRadius(float min, float max)
        {
            _spawnMinRadius = min;
            _spawnMaxRadius = max;
        }

        public void Pause()
        {
            if (_isPaused)
                return;

            _isPaused = true;
            UnityEngine.Time.timeScale = 0f;

            PausedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void Resume()
        {
            if (!_isPaused)
                return;

            _isPaused = false;
            UnityEngine.Time.timeScale = 1f;

            ResumedEvent?.Invoke(this, EventArgs.Empty);
        }

        public System.Numerics.Vector2 GetNextSpawnPoint()
        {
            var playerPos = _player.GetPosition();
            for (var i = 0; i < 3; i++)
            {
                if (TryGetRandomPositionAround(playerPos, _spawnMinRadius, _spawnMaxRadius, out var position))
                    return position;
            }
            return playerPos;
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

        private bool TryGetRandomPositionAround(System.Numerics.Vector2 center, float minDistance, float maxDistance, out System.Numerics.Vector2 position)
        {
            var candidate = Random.NextPosition(center, minDistance, maxDistance);
            var candidate3D = new Vector3(candidate.X, 0, candidate.Y);
            if (NavMesh.SamplePosition(candidate3D, out var hit, 1f, NavMesh.AllAreas))
            {
                position = new System.Numerics.Vector2(hit.position.x, hit.position.z);
                return true;
            }
            position = default;
            return false;
        }

        public System.Numerics.Vector2 GetPlayerUnitSpawnPosition()
        {
            var pos = _playerUnitSpawnPosition.transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }
    }
}
