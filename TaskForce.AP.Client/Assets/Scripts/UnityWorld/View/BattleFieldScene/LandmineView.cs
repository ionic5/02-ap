using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 지뢰의 비주얼 로직을 담당합니다. 적 감지 시 폭발 이벤트를 발생시킵니다.
    /// </summary>
    public class LandmineView : PoolableObject, IPowderKeg
    {
        public event EventHandler ExplosionEvent;
        public event EventHandler<BatchObjectDetectedEventArgs> BatchObjectDetectedEvent;

        [SerializeField]
        private float _detectInterval = 0.2f;
        private float _detectTimer;
        private float _watchRadius;
        private bool _isWatching;

        private readonly Collider[] _detectResults = new Collider[20];

        protected override void CleanUp()
        {
            base.CleanUp();
            ExplosionEvent = null;
            BatchObjectDetectedEvent = null;
            _isWatching = false;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, 0.05f, position.Y);
        }

        public void Watch(float watchRadius)
        {
            _watchRadius = watchRadius;
            _isWatching = true;
            _detectTimer = 0;
        }

        public void Ignite()
        {
            _isWatching = false;
            // 애니메이션 실행 (기존 프리팹의 애니메이터 사용 가정)
            var animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                animator.Play("ignite");
            }
            else
            {
                // 애니메이터가 없으면 즉시 폭발
                OnIgniteFinished();
            }
        }

        /// <summary>
        /// 애니메이션 이벤트 'OnIgniteFinished'의 리시버입니다.
        /// </summary>
        public void OnIgniteFinished()
        {
            ExplosionEvent?.Invoke(this, EventArgs.Empty);
        }

        private void Update()
        {
            if (!_isWatching) return;

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
                var objectIDs = new string[count];
                for (int i = 0; i < count; i++)
                {
                    objectIDs[i] = _detectResults[i].gameObject.name;
                }
                BatchObjectDetectedEvent?.Invoke(this, new BatchObjectDetectedEventArgs(objectIDs));
            }
        }

        public System.Numerics.Vector2 GetPosition()
        {
            var pos = transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }

        // 구버전 호환용 (사용 안함)
        public void Start() { }
    }
}
