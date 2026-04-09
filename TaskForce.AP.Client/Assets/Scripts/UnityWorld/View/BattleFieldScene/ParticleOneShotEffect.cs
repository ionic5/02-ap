using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 단발성 파티클 이펙트와 폭발 데미지 로직을 통합한 클래스입니다.
    /// 모든 자식 파티클 시스템을 포함하여 단발 재생을 보장합니다.
    /// </summary>
    public class ParticleOneShotEffect : PoolableObject, IExplosion
    {
        private ParticleSystem[] _allParticleSystems;

        public event EventHandler ExplosionFinishedEvent;
        public event EventHandler<BatchObjectHitEventArgs> BatchObjectHitEvent;
        
        private readonly Collider[] _hitResults = new Collider[50];
        private bool _isStarted;
        private float _safetyTimer;
        private float _maxDuration;

        private void Awake()
        {
            CacheAndPrepareParticles();
        }

        private void CacheAndPrepareParticles()
        {
            if (_allParticleSystems == null || _allParticleSystems.Length == 0)
            {
                _allParticleSystems = GetComponentsInChildren<ParticleSystem>();
            }

            foreach (var ps in _allParticleSystems)
            {
                var main = ps.main;
                main.playOnAwake = false;
                main.loop = false;
            }
        }

        // 명시적 인터페이스 구현
        void IExplosion.Start(float explosionRadius)
        {
            CacheAndPrepareParticles();

            _maxDuration = 0;
            foreach (var ps in _allParticleSystems)
            {
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                
                var main = ps.main;
                main.loop = false;
                
                ps.Play(true);
                
                // 가장 긴 재생 시간을 계산
                float duration = main.duration + main.startLifetime.constantMax;
                if (duration > _maxDuration) _maxDuration = duration;
            }

            // 데미지 범위 감지 로직
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

            _safetyTimer = 0;
            _isStarted = true;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0.5f, position.Y);
        }

        private void Update()
        {
            if (!_isStarted) return;

            _safetyTimer += UnityEngine.Time.deltaTime;

            // 모든 파티클이 종료되었는지 확인
            bool anyAlive = false;
            foreach (var ps in _allParticleSystems)
            {
                if (ps.IsAlive(true))
                {
                    anyAlive = true;
                    break;
                }
            }

            if (!anyAlive || _safetyTimer > _maxDuration + 1.0f)
            {
                Finish();
            }
        }

        private void Finish()
        {
            if (!_isStarted) return;
            _isStarted = false;
            ExplosionFinishedEvent?.Invoke(this, EventArgs.Empty);
            Destroy();
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _isStarted = false;
            ExplosionFinishedEvent = null;
            BatchObjectHitEvent = null;
            
            if (_allParticleSystems != null)
            {
                foreach (var ps in _allParticleSystems)
                {
                    ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }
    }
}
