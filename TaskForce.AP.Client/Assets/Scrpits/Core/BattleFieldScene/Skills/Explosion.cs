using System;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class Explosion
    {
        private readonly IExplosion _explosion;
        private readonly IUnit _caster;
        private readonly Random _random;
        private readonly int _minDamage;
        private readonly int _maxDamage;
        private readonly float _explosionRadius;
        private bool _isDestroyed;

        public Explosion(IExplosion explosion, IUnit caster, Random random, int minDamage, int maxDamage, float explosionRadius)
        {
            _explosion = explosion;
            _caster = caster;
            _random = random;
            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _explosionRadius = explosionRadius;

            _explosion.BatchObjectHitEvent += OnBatchObjectHitEvent;
            _explosion.ExplosionFinishedEvent += OnExplosionFinishedEvent;
            _explosion.DestroyEvent += OnDestroyEvent;
        }

        public void Start(Vector2 position)
        {
            _explosion.SetPosition(position);
            _explosion.Start(_explosionRadius);
        }

        private void OnBatchObjectHitEvent(object sender, BatchObjectHitEventArgs args)
        {
            var targets = _caster.FindTargets(args.ObjectIDs);
            for (int i = targets.Count() - 1; i >= 0; i--)
                targets.ElementAt(i).Hit(_caster, _random.Next(_minDamage, _maxDamage));
        }

        private void OnExplosionFinishedEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void OnDestroyEvent(object sender, EventArgs args)
        {
            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _explosion.Destroy();
            _explosion.DestroyEvent -= OnDestroyEvent;
            _explosion.BatchObjectHitEvent -= OnBatchObjectHitEvent;
            _explosion.ExplosionFinishedEvent -= OnExplosionFinishedEvent;
        }
    }
}
