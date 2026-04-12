using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    public class Follower : MonoBehaviour
    {
        private IFollowable _target;
        private Vector3 _offset;

        public void Follow(IFollowable target)
        {
            Follow(target, Vector3.zero);
        }

        public void Follow(IFollowable target, Vector3 offset)
        {
            _target = target;
            _offset = offset;
            _target.DestroyEvent += OnDestroyTargetEvent;
        }

        private void OnDestroyTargetEvent(object sender, DestroyEventArgs args)
        {
            Unfollow();
        }

        public void Unfollow()
        {
            if (_target == null)
                return;

            _target.DestroyEvent -= OnDestroyTargetEvent;
            _target = null;
        }

        public void FixedUpdate()
        {
            if (_target == null)
                return;

            System.Numerics.Vector2 pos = _target.GetPosition();
            transform.position = new Vector3(pos.X + _offset.x, _offset.y, pos.Y + _offset.z);
        }

        public void OnDestroy()
        {
            Unfollow();
        }
    }
}
