using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class Explosion : PoolableObject, IExplosion
    {
        [SerializeField]
        private Animator _animator;

        public event EventHandler ExplosionFinishedEvent;
        public event EventHandler<BatchObjectHitEventArgs> BatchObjectHitEvent;
        private readonly Collider[] _hitResults = new Collider[50];

        void IExplosion.Start(float explosionRadius)
        {
            _animator.Play("explosion");

            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                explosionRadius,
                _hitResults
            );

            if (hitCount > 0)
            {
                var hitNames = _hitResults.Take(hitCount).Select(c => c.gameObject.name);
                BatchObjectHitEvent?.Invoke(this, new BatchObjectHitEventArgs(hitNames));

                Array.Clear(_hitResults, 0, _hitResults.Length);
            }
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0.0f, position.Y);
        }

        public void OnExplosionFinished()
        {
            ExplosionFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            ExplosionFinishedEvent = null;
            BatchObjectHitEvent = null;
        }
    }
}
