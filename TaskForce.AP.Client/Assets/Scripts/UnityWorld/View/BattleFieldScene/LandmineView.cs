using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 지뢰의 비주얼 로직을 담당합니다. 적 감지 시 즉각 폭발합니다.
    /// </summary>
    public class LandmineView : PoolableObject, IPowderKeg
    {
        public event EventHandler ExplosionEvent;
        public event EventHandler<BatchObjectDetectedEventArgs> BatchObjectDetectedEvent;

        [SerializeField]
        private float _detectInterval = 0.1f; // 감지 주기를 더 짧게 하여 반응성 향상
        [SerializeField]
        private float _armingDelay = 0.3f;    // 설치 후 약간의 유예 시간을 더 줌 (지연 시간을 줄여서 반응성 개선)

        private float _detectTimer;
        private float _activationTimer;
        private float _watchRadius;
        private bool _isWatching;
        private bool _isArmed;

        private readonly Collider[] _detectResults = new Collider[20];

        protected override void CleanUp()
        {
            base.CleanUp();
            ExplosionEvent = null;
            BatchObjectDetectedEvent = null;
            _isWatching = false;
            _isArmed = false;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0.1f, position.Y);
        }

        public void Watch(float watchRadius)
        {
            _watchRadius = watchRadius;
            _isWatching = true;
            _isArmed = false;
            _detectTimer = 0;
            _activationTimer = 0;
        }

        public void Ignite()
        {
            if (!_isWatching) return;
            _isWatching = false;
            _isArmed = false;

            // 1. 비주얼 애니메이션은 시작만 함 (끝날 때까지 기다리지 않음)
            var animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Play("ignite");
            }

            // 2. 로직상의 폭발 이벤트는 '즉시' 발생시킴 (사용자 피드백 반영)
            ExplosionEvent?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 애니메이션 이벤트 'OnIgniteFinished' 에러 방지용 더미 메서드입니다.
        /// </summary>
        public void OnIgniteFinished()
        {
            // 아무것도 하지 않음 (이미 Ignite에서 폭발 처리를 했기 때문)
        }

        private void Update()
        {
            if (!_isWatching) return;

            // 활성화 유예 시간
            if (!_isArmed)
            {
                _activationTimer += UnityEngine.Time.deltaTime;
                if (_activationTimer >= _armingDelay)
                {
                    _isArmed = true;
                }
                return;
            }

            _detectTimer += UnityEngine.Time.deltaTime;
            if (_detectTimer >= _detectInterval)
            {
                _detectTimer = 0;
                DetectEnemies();
            }
        }

        private void DetectEnemies()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, _watchRadius, _detectResults);
            
            if (count > 0)
            {
                var enemyIDs = _detectResults
                    .Take(count)
                    .Where(c => c != null)
                    .Select(c => {
                        // 콜라이더가 자식 객체에 있을 경우를 대비하여 부모 유닛을 찾음
                        var unit = c.GetComponentInParent<Unit>();
                        return unit != null ? unit.gameObject.name : c.gameObject.name;
                    })
                    .ToArray();

                if (enemyIDs.Length > 0)
                {
                    BatchObjectDetectedEvent?.Invoke(this, new BatchObjectDetectedEventArgs(enemyIDs));
                }
            }
        }

        public System.Numerics.Vector2 GetPosition()
        {
            var pos = transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }

        public void Start() { }
    }
}
