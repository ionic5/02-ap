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
        private readonly RaycastHit[] _groundHits = new RaycastHit[20]; // Raycast 결과 캐싱 배열

        protected override void CleanUp()
        {
            base.CleanUp();
            ExplosionEvent = null;
            BatchObjectDetectedEvent = null;
            _isWatching = false;
            _isArmed = false;
            transform.rotation = Quaternion.identity; // 풀 반환 시 회전 초기화
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            float targetY = 0.1f; 
            
            // 10m 위에서 아래로 Ray를 쏨
            Vector3 origin = new Vector3(position.X, 10f, position.Y);
            
            // NonAlloc 사용 및 트리거 콜라이더 무시
            int hitCount = Physics.RaycastNonAlloc(origin, Vector3.down, _groundHits, 20f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
            bool foundGround = false;
            RaycastHit bestHit = default;
            float maxY = float.MinValue;

            for (int i = 0; i < hitCount; i++)
            {
                var hit = _groundHits[i];
                // 동적 오브젝트(유닛, 아이템, 지뢰 등)는 PoolableObject를 가지고 있으므로 제외하여 순수 지형만 찾음
                if (hit.collider.GetComponentInParent<PoolableObject>() != null) continue;
                
                if (hit.point.y > maxY)
                {
                    maxY = hit.point.y;
                    bestHit = hit;
                    foundGround = true;
                }
            }

            if (foundGround)
            {
                targetY = bestHit.point.y + 0.02f;
                // 바닥의 기울기(법선 벡터)에 맞춰 지뢰의 방향을 회전시킴
                transform.up = bestHit.normal;
            }
            else
            {
                // 2. 지면 감지 실패 시, 캐싱된 활성 유닛 리스트에서 첫 번째 유닛의 높이를 참고
                var fallbackUnit = Unit.ActiveUnits.FirstOrDefault();
                    
                if (fallbackUnit != null)
                {
                    targetY = fallbackUnit.transform.position.y + 0.02f;
                }
                // 감지 실패 시 기본 회전으로 설정
                transform.rotation = Quaternion.identity;
            }

            transform.position = new Vector3(position.X, targetY, position.Y);
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
                        // 구체적인 Unit 클래스 대신 IUnit 인터페이스를 통해 유닛 식별
                        var unit = c.GetComponentInParent<TaskForce.AP.Client.Core.View.BattleFieldScene.IUnit>();
                        return unit is Component comp ? comp.gameObject.name : c.gameObject.name;
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
