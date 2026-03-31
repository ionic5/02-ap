using System;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class SheepMissile
    {
        private readonly Core.Random _random;
        private readonly IMissile _sheep;
        private readonly IUnit _caster;
        private readonly int _minDamage;
        private readonly int _maxDamage;
        private readonly ITargetFinder _targetFinder;
        private bool _isDestroyed;

        public SheepMissile(Random random, IMissile sheep, IUnit caster, int minDamage, int maxDamage, ITargetFinder targetFinder)
        {
            _isDestroyed = false;
            _random = random;
            _sheep = sheep;
            _caster = caster;
            _sheep.ArrivedDestinationEvent += OnArrivedDestinationEvent;
            _sheep.DestroyEvent += OnDestroyEvent;
            _sheep.HitEvent += OnHitEvent;
            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _targetFinder = targetFinder;
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        private void OnArrivedDestinationEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _sheep.Destroy();
            _sheep.ArrivedDestinationEvent -= OnArrivedDestinationEvent;
            _sheep.DestroyEvent -= OnDestroyEvent;
        }

        public void MoveTo(Vector2 destination, float speed)
        {
            _sheep.MoveTo(destination, speed);
        }

        public void SetPosition(Vector2 position)
        {
            _sheep.SetPosition(position);
        }

        private void OnHitEvent(object sender, View.HitEventArgs args)
        {
            var target = _targetFinder.FindByViewID(args.ObjectID);
            if (target == null)
                return;

            target.Hit(_caster, _random.Next(_minDamage, _maxDamage));
        }
    }
}
