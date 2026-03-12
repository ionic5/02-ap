using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class OneShotEffect : PoolableObject, IOneShotEffect
    {
        [SerializeField]
        private Animator _animator;

        [SerializeField]
        private Follower _follower;

        public void Play()
        {
            _animator.Play("default");
        }

        public void SetPosition(System.Numerics.Vector2 pos)
        {
            transform.position = new Vector3(pos.X, 0, pos.Y);
        }

        public void OnAnimationFinished()
        {
            Destroy();
        }

        public void Follow(IFollowable target)
        {
            _follower.Follow(target);
        }
    }
}
