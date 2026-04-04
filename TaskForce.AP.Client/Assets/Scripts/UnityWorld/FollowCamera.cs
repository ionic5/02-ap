using TaskForce.AP.Client.Core.View.BattleFieldScene;
using Unity.Cinemachine;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
    public class FollowCamera : MonoBehaviour, IFollowCamera
    {
        [SerializeField]
        private CinemachineCamera _camera;
        [SerializeField]
        private GameObject _targetProxy;

        private Vector3 _position;
        private Core.BattleFieldScene.IFollowable _target;

        private void Awake()
        {
            _position = new Vector3();
            _target = null;
        }

        public void SetTarget(Core.BattleFieldScene.IFollowable _unit)
        {
            _target = _unit;

            UpdateTargetProxy();
        }

        public void UnsetTarget()
        {
            _target = null;
        }

        void LateUpdate()
        {
            UpdateTargetProxy();
        }

        private void UpdateTargetProxy()
        {
            if (_target != null)
            {
                System.Numerics.Vector2 position = _target.GetPosition();
                _position.x = position.X;
                _position.z = position.Y;
            }

            _targetProxy.transform.position = _position;
        }
    }
}