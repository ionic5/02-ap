using TaskForce.AP.Client.Core.View.BattleFieldScene;
using System;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class Sheep : PoolableObject, IMissile
    {
        public event EventHandler ArrivedDestinationEvent;
        public event EventHandler<Core.View.HitEventArgs> HitEvent;

        private Vector3 _destination;
        private bool _hasDestination;

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _arrivalThreshold;

        private void Awake()
        {
            _isDestroyed = false;
            _hasDestination = false;
            _destination = Vector3.zero;
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            ArrivedDestinationEvent = null;
            HitEvent = null;
        }

        public void MoveTo(System.Numerics.Vector2 destination, float speed)
        {
            _hasDestination = true;
            _destination = new Vector3(destination.X, 0, destination.Y);

            Vector3 direction = (_destination - transform.position).normalized;
            _rigidbody.linearVelocity = direction * speed;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0, position.Y);
        }

        private void Update()
        {
            if (!_hasDestination)
                return;

            if (!IsArrived())
                return;

            _hasDestination = false;
            _rigidbody.linearVelocity = Vector3.zero;
            _destination = Vector3.zero;

            ArrivedDestinationEvent?.Invoke(this, EventArgs.Empty);
        }

        private bool IsArrived()
        {
            Vector3 diff = _destination - _rigidbody.position;
            float sqrDist = diff.sqrMagnitude;
            return sqrDist < _arrivalThreshold * _arrivalThreshold;
        }

        private void OnTriggerEnter(Collider other)
        {
            HitEvent?.Invoke(this, new Core.View.HitEventArgs { ObjectID = other.gameObject.name });
        }

        public System.Numerics.Vector2 GetPosition()
        {
            var pos = transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }
    }
}