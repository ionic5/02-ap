using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class Soul : IDestroyable
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        private bool _isDestroyed;
        private readonly Core.View.BattleFieldScene.ISoul _soulView;
        private readonly int _level;
        private BattleFieldScene.IFollowable _followTarget;

        public Soul(View.BattleFieldScene.ISoul soulView, int level)
        {
            _soulView = soulView;
            _level = level;

            _soulView.DestroyEvent += OnDestroyEvent;
        }

        public void SetPosition(Vector2 position)
        {
            _soulView.SetPosition(position);
        }

        public bool IsMovingTo(Core.BattleFieldScene.IFollowable followTarget)
        {
            return _followTarget == followTarget;
        }

        public void MoveTo(BattleFieldScene.IFollowable followTarget, float speed)
        {
            if (_followTarget != null)
                UnsetFollowTarget();

            _followTarget = followTarget;

            _followTarget.DestroyEvent += OnDestroyUnitEvent;
            _soulView.MoveTo(_followTarget, speed);
        }

        private void UnsetFollowTarget()
        {
            if (_followTarget == null)
                return;

            _followTarget.DestroyEvent -= OnDestroyUnitEvent;
            _followTarget = null;
        }

        private void OnDestroyUnitEvent(object sender, DestroyEventArgs args)
        {
            if (args.TargetObject != _followTarget)
                return;

            UnsetFollowTarget();
            _soulView.Stop();
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        public Vector2 GetPosition()
        {
            return _soulView.GetPosition();
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            _isDestroyed = true;

            UnsetFollowTarget();

            _soulView.DestroyEvent -= OnDestroyEvent;
            _soulView.Destroy();
        }

        public int GetLevel()
        {
            return _level;
        }
    }
}
