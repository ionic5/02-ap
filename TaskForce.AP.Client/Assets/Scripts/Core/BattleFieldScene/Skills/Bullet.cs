using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using System.Numerics;
using TaskForce.AP.Client.Core.View;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 총알(투사체)의 코어 로직을 담당합니다. 이동 및 충돌 처리를 수행합니다.
    /// </summary>
    public class Bullet : IUpdatable, IDestroyable
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        private readonly Core.Random _random;
        private readonly IMissile _view; 
        private readonly ITargetFinder _targetFinder;
        private readonly IUnit _caster;
        private ITarget _target;
        private int _minDamage;
        private int _maxDamage;
        private bool _isDestroyed;

        public Bullet(Core.Random random, IMissile view, ITargetFinder targetFinder, IUnit caster, int minDamage, int maxDamage)
        {
            _random = random;
            _view = view;
            _targetFinder = targetFinder;
            _caster = caster;
            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _isDestroyed = false;

            _view.HitEvent += OnHitEvent;
        }

        public void SetPosition(Vector2 position)
        {
            _view.SetPosition(position);
        }

        public void SetTarget(ITarget target)
        {
            _target = target;
            _view.SetTarget(target.GetViewID());
        }

        public void SetSpeed(float speed)
        {
            _view.SetSpeed(speed);
        }

        public void Start()
        {
            _view.Start();
        }

        public void Update()
        {
            // 필요한 경우 사거리 체크 등을 수행할 수 있습니다.
        }

        private void OnHitEvent(object sender, HitEventArgs e)
        {
            // View에서 전달받은 ObjectID로 실제 로직 타겟 식별
            var hitTarget = _targetFinder.FindByViewID(e.ObjectID);
            
            // 타겟이 존재하면 데미지 적용
            if (hitTarget != null)
            {
                var damage = _random.Next(_minDamage, _maxDamage + 1);
                hitTarget.Hit(_caster, damage);
            }

            Destroy();
        }

        public void Destroy()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;

            _view.HitEvent -= OnHitEvent;
            _view.Destroy();
            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
        }
    }
}