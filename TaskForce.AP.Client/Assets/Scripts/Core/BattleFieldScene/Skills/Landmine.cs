using System;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    /// <summary>
    /// 지뢰 로직을 담당합니다. 적이 감지 범위 내에 들어오면 폭발합니다.
    /// </summary>
    public class Landmine
    {
        private bool _isDestroyed;

        private readonly IPowderKeg _landmineView; // 기존 IPowderKeg 인터페이스를 그대로 활용
        private readonly IUnit _caster;

        private readonly int _minDamage;
        private readonly int _maxDamage;
        private readonly float _explosionRadius;
        private readonly float _watchRadius;
        private readonly float _expireTime;
        private readonly Func<IUnit, int, int, float, Explosion> _createExplosion;
        private readonly Timer _timer;

        public Landmine(IPowderKeg landmineView, IUnit caster, Timer expireTimer,
            int minDamage, int maxDamage, float watchRadius, float explosionRadius, float expireTime,
            Func<IUnit, int, int, float, Explosion> createExplosion)
        {
            _landmineView = landmineView;
            _caster = caster;
            _timer = expireTimer;

            _minDamage = minDamage;
            _maxDamage = maxDamage;
            _watchRadius = watchRadius;
            _explosionRadius = explosionRadius;
            _createExplosion = createExplosion;
            _expireTime = expireTime;

            _landmineView.DestroyEvent += OnDestroyEvent;
            _landmineView.BatchObjectDetectedEvent += OnObjectDetectedEvent;
            _landmineView.ExplosionEvent += OnExplosionEvent;
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        public void Plant(Vector2 position)
        {
            _landmineView.SetPosition(position);
            _landmineView.Watch(_watchRadius);

            // 지뢰는 수명이 매우 길거나 적이 닿을 때까지 유지되지만, 
            // 시스템 안정성을 위해 아주 긴 시간이 지나면 터지도록 설정할 수 있습니다.
            if (_expireTime > 0)
            {
                _timer.Start(_expireTime, () => { Ignite(); });
            }
        }

        private void OnObjectDetectedEvent(object sender, BatchObjectDetectedEventArgs args)
        {
            // 감지된 오브젝트 중 적이 하나라도 있으면 즉시 폭발
            var targets = _caster.FindTargets(args.ObjectIDs);
            if (targets.Any())
            {
                Ignite();
            }
        }

        private void Ignite()
        {
            _timer.Stop();
            _landmineView.Ignite();
        }

        private void OnExplosionEvent(object sender, EventArgs eventArgs)
        {
            // 폭발 이펙트 및 범위 데미지 생성
            var explosion = _createExplosion(_caster, _minDamage, _maxDamage, _explosionRadius);
            explosion.Start(_landmineView.GetPosition());

            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            _timer.Destroy();
            _landmineView.Destroy();
            _landmineView.DestroyEvent -= OnDestroyEvent;
            _landmineView.BatchObjectDetectedEvent -= OnObjectDetectedEvent;
            _landmineView.ExplosionEvent -= OnExplosionEvent;
        }
    }
}
