using System;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class Dynamite
    {
        private bool _isDestroyed;

        private readonly IMissile _dynamite;
        private readonly IUnit _caster;
        private readonly int _minDamage;
        private readonly int _maxDamage;
        private readonly float _explosionRadius;
        private readonly Func<IUnit, int, int, float, Explosion> _createExplosion;

        public Dynamite(IMissile dynamite, IUnit caster,
            int minDamage, int maxDamage, float explosionRadius,
            Func<IUnit, int, int, float, Explosion> createExplosion)
        {
            _isDestroyed = false;
            _dynamite = dynamite;
            _caster = caster;
            _dynamite.ArrivedDestinationEvent += OnArrivedDestinationEvent;
            _dynamite.DestroyEvent += OnDestroyEvent;

            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _explosionRadius = explosionRadius;
            _createExplosion = createExplosion;
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        private void OnArrivedDestinationEvent(object sender, EventArgs e)
        {
            var explosion = _createExplosion(_caster, _minDamage, _maxDamage, _explosionRadius);
            explosion.Start(_dynamite.GetPosition());

            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _dynamite.Destroy();
            _dynamite.ArrivedDestinationEvent -= OnArrivedDestinationEvent;
            _dynamite.DestroyEvent -= OnDestroyEvent;
        }

        public void MoveTo(Vector2 destination, float speed)
        {
            _dynamite.MoveTo(destination, speed);
        }

        public void SetPosition(Vector2 position)
        {
            _dynamite.SetPosition(position);
        }
    }
}
