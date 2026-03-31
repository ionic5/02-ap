using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class Soul : PoolableObject, Core.View.BattleFieldScene.ISoul
    {
        private Core.BattleFieldScene.IFollowable _followTarget;
        private Vector3 _unitPosition;
        private System.Numerics.Vector2 _position;
        private float _speed;

        private void Awake()
        {
            _unitPosition = new Vector3();
        }

        public System.Numerics.Vector2 GetPosition()
        {
            _position.X = transform.position.x;
            _position.Y = transform.position.z;

            return _position;
        }

        public void MoveTo(Core.BattleFieldScene.IFollowable followTarget, float speed)
        {
            _followTarget = followTarget;
            _speed = speed;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, transform.position.y, position.Y);
        }

        public void Stop()
        {
            _followTarget = null;
        }

        private void Update()
        {
            if (_followTarget == null)
                return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                GetUnitPosition(),
                _speed * UnityEngine.Time.deltaTime
            );
        }

        private Vector3 GetUnitPosition()
        {
            var pos = _followTarget.GetPosition();

            _unitPosition.x = pos.X;
            _unitPosition.z = pos.Y;
            _unitPosition.y = transform.position.y;

            return _unitPosition;
        }
    }
}
