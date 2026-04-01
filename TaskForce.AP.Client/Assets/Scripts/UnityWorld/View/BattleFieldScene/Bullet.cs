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

        private IUnit _target;
        private Core.Timer _destroyTimer;
        private bool _isMoving;
        private string _targetViewID;

        public void SetSpeed(float speed) => _moveSpeed = speed;

        public void SetTarget(string targetViewID)
        {
            _targetViewID = targetViewID;
            // 타겟 뷰 객체를 직접 찾아 연결
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
        }

        public override void Revive()
        {
            base.Revive();
            _isMoving = false;
            _target = null;
            _targetViewID = null;
            _destroyTimer = new Core.Timer(_coreTime, _coreLoop);
            _destroyTimer.Start(_destroyDelay, () => Destroy());
        }

        private void Update()
        {
            // 타겟이 없거나 죽었으면 파괴
            if (!_isMoving || _target == null)
            {
                if (_isMoving) Destroy();
                return;
            }

            Vector3 targetPos = new Vector3(_target.GetPosition().X, transform.position.y, _target.GetPosition().Y);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, _moveSpeed * UnityEngine.Time.deltaTime);

            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(direction);

            if (Vector3.Distance(transform.position, targetPos) < 0.2f)
            {
                OnHit();
            }
        }

        private void OnHit()
        {
            _isMoving = false;
            HitEvent?.Invoke(this, new HitEventArgs { ObjectID = _targetViewID });
            Destroy();
        }

        protected override void OnDestroy()
        {
            _destroyTimer?.Stop();
            _isMoving = false;
            _target = null;
            _targetViewID = null;
            HitEvent = null;
            ArrivedDestinationEvent = null;
            base.OnDestroy();
        }
    }
}