using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class PowderKeg : PoolableObject, IPowderKeg
    {
        public event EventHandler<BatchObjectDetectedEventArgs> BatchObjectDetectedEvent;
        public event EventHandler ExplosionEvent;

        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private float _watchInterval;
        [SerializeField]
        private float _minBlinkInterval;
        [SerializeField]
        private float _maxBlinkInterval;

        private bool _isWatching;
        private float _watchRadius;
        private float _lastCheckTime;
        private float _nextBlinkTime;

        private readonly Collider[] _overlapResults = new Collider[20];

        protected override void CleanUp()
        {
            base.CleanUp();

            BatchObjectDetectedEvent = null;
            ExplosionEvent = null;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0, position.Y);
        }

        public System.Numerics.Vector2 GetPosition()
        {
            var pos = transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }

        public void Watch(float watchRadius)
        {
            _isWatching = true;
            _watchRadius = watchRadius;
            _lastCheckTime = 0.0f;

            _animator.Play("watch");
            DetermineNextBlinkTime();
        }

        public void Ignite()
        {
            _isWatching = false;

            _animator.Play("ignite");
        }

        public void OnIgniteFinished()
        {
            ExplosionEvent?.Invoke(this, EventArgs.Empty);
        }

        private void FixedUpdate()
        {
            if (!_isWatching)
                return;

            if (UnityEngine.Time.time >= _nextBlinkTime)
            {
                _animator.SetTrigger("blink");
                DetermineNextBlinkTime();
            }

            if (UnityEngine.Time.fixedTime - _lastCheckTime < _watchInterval) return;
            _lastCheckTime = UnityEngine.Time.fixedTime;

            int hitCount = Physics.OverlapSphereNonAlloc(transform.position, _watchRadius, _overlapResults);
            if (hitCount > 0)
            {
                var detectedNames = _overlapResults.Take(hitCount).Select(c => c.gameObject.name);
                BatchObjectDetectedEvent?.Invoke(this, new BatchObjectDetectedEventArgs(detectedNames));
                
                Array.Clear(_overlapResults, 0, _overlapResults.Length);
            }
        }

        private void DetermineNextBlinkTime()
        {
            _nextBlinkTime = UnityEngine.Time.time + UnityEngine.Random.Range(_minBlinkInterval, _maxBlinkInterval);
        }
    }
}
