using TaskForce.AP.Client.Core.View.BattleFieldScene;
using System;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    /// <summary>
    /// 수류탄의 비주얼 로직을 담당합니다. 포물선 투척 효과를 구현합니다.
    /// </summary>
    public class GrenadeView : PoolableObject, IMissile
    {
        public event EventHandler ArrivedDestinationEvent;
        public event EventHandler<Core.View.HitEventArgs> HitEvent;

        private Vector3 _startPosition;
        private Vector3 _destination;
        private bool _isMoving;
        private float _timeElapsed;
        private float _duration;

        [SerializeField]
        private float _arcHeight = 2.0f; // 포물선 높이

        protected override void CleanUp()
        {
            base.CleanUp();
            ArrivedDestinationEvent = null;
            HitEvent = null;
            _isMoving = false;
        }

        public void MoveTo(System.Numerics.Vector2 destination, float speed)
        {
            _startPosition = transform.position;
            _destination = new Vector3(destination.X, _startPosition.y, destination.Y);
            
            float distance = Vector3.Distance(_startPosition, _destination);
            _duration = distance / speed;
            
            _timeElapsed = 0;
            _isMoving = true;
        }

        public void SetTarget(string targetViewID) { }

        public void SetSpeed(float speed) { }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, transform.position.y, position.Y);
        }

        public void Start() { }

        private void Update()
        {
            if (!_isMoving) return;

            _timeElapsed += UnityEngine.Time.deltaTime;
            float progress = _timeElapsed / _duration;

            if (progress >= 1.0f)
            {
                transform.position = _destination;
                _isMoving = false;
                ArrivedDestinationEvent?.Invoke(this, EventArgs.Empty);
                return;
            }

            // X, Z축 선형 보간
            Vector3 currentPos = Vector3.Lerp(_startPosition, _destination, progress);

            // Y축 포물선 계산 (sin curve)
            float yOffset = Mathf.Sin(progress * Mathf.PI) * _arcHeight;
            currentPos.y += yOffset;

            transform.position = currentPos;
        }

        private void OnTriggerEnter(Collider other)
        {
            HitEvent?.Invoke(this, new Core.View.HitEventArgs { ObjectID = other.gameObject.name });
        }

        public System.Numerics.Vector2 GetPosition()
        {
            var pos = transform.position;
            return new System.Numerics.Vector2(pos.x, pos.z);
        }
    }
}
