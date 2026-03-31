using System;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class PowderKeg
    {
        private bool _isDestroyed;

        private readonly IPowderKeg _powderKeg;
        private readonly IUnit _caster;

        private readonly int _minDamage;
        private readonly int _maxDamage;
        private readonly float _explosionRadius;
        private readonly float _watchRadius;
        private readonly float _expireTime;
        private readonly Func<IUnit, int, int, float, Explosion> _createExplosion;
        private readonly Timer _timer;

        public PowderKeg(IPowderKeg powderKeg, IUnit caster, Timer expireTimer,
            int minDamage, int maxDamage, float watchRadius, float explosionRadius, float expireTime,
            Func<IUnit, int, int, float, Explosion> createExplosion)
        {
            _powderKeg = powderKeg;
            _caster = caster;
            _timer = expireTimer;

            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _watchRadius = watchRadius;
            _explosionRadius = explosionRadius;
            _createExplosion = createExplosion;
            _expireTime = expireTime;

            _powderKeg.DestroyEvent += OnDestroyEvent;
            _powderKeg.BatchObjectDetectedEvent += OnObjectDetectedEvent;
            _powderKeg.ExplosionEvent += OnExplosionEvent;
        }

        private void OnExpireTimeOutEvent(object sender, EventArgs e)
        {
            Ignite();
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        public void Plant(Vector2 position)
        {
            _powderKeg.SetPosition(position);
            _powderKeg.Watch(_watchRadius);

            _timer.Start(_expireTime, () => { Ignite(); });
        }

        private void OnObjectDetectedEvent(object sender, BatchObjectDetectedEventArgs args)
        {
            var targets = _caster.FindTargets(args.ObjectIDs);
            if (targets.Any())
                Ignite();
        }

        private void Ignite()
        {
            _timer.Stop();

            _powderKeg.Ignite();
        }

        private void OnExplosionEvent(object sender, EventArgs eventArgs)
        {
            var explosion = _createExplosion(_caster, _minDamage, _maxDamage, _explosionRadius);
            explosion.Start(_powderKeg.GetPosition());

            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _timer.Destroy();
            _powderKeg.Destroy();
            _powderKeg.DestroyEvent -= OnDestroyEvent;
            _powderKeg.BatchObjectDetectedEvent -= OnObjectDetectedEvent;
            _powderKeg.ExplosionEvent -= OnExplosionEvent;
        }
    }
}
