using System;
using UnityEngine;
using TaskForce.AP.Client.Core.View;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using IUnit = TaskForce.AP.Client.Core.View.BattleFieldScene.IUnit;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// Unity 환경에서 총알의 시각적 표현 및 움직임을 담당합니다.
    /// </summary>
    public class Bullet : UnityWorld.PoolableObject, IMissile
    {
        public event EventHandler ArrivedDestinationEvent;
        public event EventHandler<HitEventArgs> HitEvent;

        [SerializeField]
        private float _moveSpeed = 15f;
        [SerializeField]
        private float _destroyDelay = 3f;
        [SerializeField]
        private float _afterHitLifeTime = 2.0f; // 효과음 재생 유지를 위한 지연 시간

        private IUnit _target;
        private Core.Timer _destroyTimer;
        private bool _isMoving;
        private bool _isHit;
        private bool _isDelayingDestruction; // 파괴 지연 중인지 여부
        private string _targetViewID;

        private Renderer[] _renderers;
        private TrailRenderer[] _trails;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _trails = GetComponentsInChildren<TrailRenderer>();
        }

        public void SetSpeed(float speed) => _moveSpeed = speed;

        public void SetTarget(string targetViewID)
        {
            _targetViewID = targetViewID;
            if (!string.IsNullOrEmpty(_targetViewID))
            {
                var targetGo = GameObject.Find(_targetViewID);
                if (targetGo != null)
                {
                    _target = targetGo.GetComponent<IUnit>();
                }
            }
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, transform.position.y, position.Y);
        }

        public System.Numerics.Vector2 GetPosition()
        {
            return new System.Numerics.Vector2(transform.position.x, transform.position.z);
        }

        public void MoveTo(System.Numerics.Vector2 destination, float speed)
        {
            _moveSpeed = speed;
        }

        public void Start()
        {
            _isMoving = true;
            _isHit = false;
            _isDelayingDestruction = false;
            SetVisualVisible(true);

            // 시작 시 단 한 번 타겟 방향을 바라보도록 설정
            if (_target != null)
            {
                Vector3 targetPos = new Vector3(_target.GetPosition().X, transform.position.y, _target.GetPosition().Y);
                Vector3 direction = (targetPos - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }
            }

            EnsureTimerInitialized();
            _destroyTimer?.Start(_destroyDelay, () => Destroy());
        }

        public override void Revive()
        {
            base.Revive();
            _isMoving = false;
            _isHit = false;
            _isDelayingDestruction = false;
            _target = null;
            _targetViewID = null;
            
            _destroyTimer?.Stop();
            EnsureTimerInitialized();
            _destroyTimer?.Start(_destroyDelay, () => Destroy());
            
            SetVisualVisible(true);
        }

        private void EnsureTimerInitialized()
        {
            if (_destroyTimer == null && _coreTime != null && _coreLoop != null)
            {
                _destroyTimer = new Core.Timer(_coreTime, _coreLoop);
            }
        }

        private void SetVisualVisible(bool visible)
        {
            if (_renderers != null)
            {
                foreach (var r in _renderers) r.enabled = visible;
            }
            if (_trails != null)
            {
                foreach (var t in _trails)
                {
                    if (visible) t.Clear();
                    t.enabled = visible;
                }
            }
        }

        private void Update()
        {
            if (_isHit || _isDelayingDestruction) return;

            if (!_isMoving || _target == null)
            {
                if (_isMoving) Destroy();
                return;
            }

            Vector3 targetPos = new Vector3(_target.GetPosition().X, transform.position.y, _target.GetPosition().Y);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * UnityEngine.Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            {
                OnHit();
            }
        }

        private void OnHit()
        {
            if (_isHit || _isDelayingDestruction) return;
            _isHit = true;

            HitEvent?.Invoke(this, new HitEventArgs { ObjectID = _targetViewID });
            Destroy(); 
        }

        protected override void HandleDestroy()
        {
            // 이미 지연 프로세스가 시작되었다면 중복 요청 무시
            if (_isDelayingDestruction) return;

            _isDelayingDestruction = true;
            _isMoving = false;

            // 시각적 요소만 즉시 숨김
            SetVisualVisible(false);

            // 타이머를 사용하여 실제 풀 반환을 지연
            _destroyTimer?.Stop();
            EnsureTimerInitialized();

            if (_destroyTimer != null)
            {
                // _afterHitLifeTime 후에 base.HandleDestroy()를 직접 호출하여 즉시 파괴(반환) 수행
                _destroyTimer.Start(_afterHitLifeTime, () => base.HandleDestroy()); 
            }
            else
            {
                base.HandleDestroy();
            }
        }

        protected override void OnDestroy()
        {
            _destroyTimer?.Stop();
            _isMoving = false;
            _isHit = false;
            _isDelayingDestruction = false;
            _target = null;
            _targetViewID = null;
            HitEvent = null;
            ArrivedDestinationEvent = null;
            base.OnDestroy();
        }
    }
}