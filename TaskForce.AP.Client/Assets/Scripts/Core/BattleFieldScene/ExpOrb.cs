using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class ExpOrb : IFieldObject
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;

        private bool _isDestroyed;
        private readonly View.BattleFieldScene.IExpOrb _view;
        private readonly int _level;
        private IFollowable _followTarget;

        public ExpOrb(View.BattleFieldScene.IExpOrb view, int level)
        {
            _view = view;
            _level = level;
            _view.DestroyEvent += OnViewDestroyEvent;
        }

        public void Handle(IFieldObjectHandler handler)
        {
            handler.Handle(this);
        }

        public Vector2 GetPosition()
        {
            return _view.GetPosition();
        }

        public void SetPosition(Vector2 position)
        {
            _view.SetPosition(position);
        }

        public bool IsMovingTo(IFollowable followTarget)
        {
            return _followTarget == followTarget;
        }

        public void MoveTo(IFollowable followTarget, float speed)
        {
            if (_followTarget != null)
                UnsetFollowTarget();

            _followTarget = followTarget;
            _followTarget.DestroyEvent += OnDestroyFollowTargetEvent;
            _view.MoveTo(_followTarget, speed);
        }

        public int GetLevel()
        {
            return _level;
        }

        private void UnsetFollowTarget()
        {
            if (_followTarget == null)
                return;

            _followTarget.DestroyEvent -= OnDestroyFollowTargetEvent;
            _followTarget = null;
        }

        private void OnDestroyFollowTargetEvent(object sender, DestroyEventArgs args)
        {
            if (args.TargetObject != _followTarget)
                return;

            UnsetFollowTarget();
            _view.Stop();
        }

        private void OnViewDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            _isDestroyed = true;

            UnsetFollowTarget();
            _view.DestroyEvent -= OnViewDestroyEvent;
            _view.Destroy();
        }
    }
}
