using System;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class Rpg
    {
        private bool _isDestroyed;

        private readonly IMissile _grenade;
        private readonly IUnit _caster;
        private readonly int _minDamage;
        private readonly int _maxDamage;
        private readonly float _explosionRadius;
        private readonly Func<IUnit, int, int, float, Explosion> _createExplosion;

        public Rpg(IMissile grenade, IUnit caster,
            int minDamage, int maxDamage, float explosionRadius,
            Func<IUnit, int, int, float, Explosion> createExplosion)
        {
            _isDestroyed = false;
            _grenade = grenade;
            _caster = caster;
            _grenade.ArrivedDestinationEvent += OnArrivedDestinationEvent;
            _grenade.DestroyEvent += OnDestroyEvent;

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
            explosion.Start(_grenade.GetPosition());

            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _grenade.Destroy();
            _grenade.ArrivedDestinationEvent -= OnArrivedDestinationEvent;
            _grenade.DestroyEvent -= OnDestroyEvent;
        }

        public void MoveTo(Vector2 destination, float speed)
        {
            _grenade.MoveTo(destination, speed);
        }

        public void SetPosition(Vector2 position)
        {
            _grenade.SetPosition(position);
        }
    }
}
