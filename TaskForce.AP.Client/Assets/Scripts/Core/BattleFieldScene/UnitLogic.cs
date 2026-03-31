using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public abstract class UnitLogic : IUnitLogic, IUpdatable
    {
        private readonly ILoop _loop;
        private IControllableUnit _unit;
        private bool _isDestroyed;
        private bool _isOnControl;

        public event EventHandler<DestroyEventArgs> DestroyEvent;

        protected UnitLogic(ILoop loop)
        {
            _loop = loop;
            _isOnControl = false;
            _isDestroyed = false;
        }

        public void StartControl(IControllableUnit unit)
        {
            _loop.Add(this);
            _isOnControl = true;
            _unit = unit;
        }

        protected IControllableUnit GetControlTarget()
        {
            return _unit;
        }

        protected bool IsDestroyed()
        {
            return _isDestroyed;
        }

        public void Update()
        {
            if (IsDestroyed() || _unit.IsDead())
                return;

            OnUpdate();
        }

        protected abstract void OnUpdate();

        public void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            if (_isOnControl)
                _loop.Remove(this);

            _unit = null;

            OnDestroy();
        }

        protected virtual void OnDestroy()
        {
        }
    }
}
