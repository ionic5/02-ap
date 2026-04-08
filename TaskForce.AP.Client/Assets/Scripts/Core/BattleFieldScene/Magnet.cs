using System;
using System.Collections.Generic;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class Magnet : IFieldItem
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;
        public event EventHandler SpawnCompletedEvent;

        private bool _isDestroyed;
        private readonly View.BattleFieldScene.IFieldItem _view;
        private readonly IExpOrbFinder _expOrbFinder;

        public Magnet(View.BattleFieldScene.IFieldItem view, IExpOrbFinder expOrbFinder)
        {
            _view = view;
            _expOrbFinder = expOrbFinder;
            _view.DestroyEvent += OnViewDestroyEvent;
            _view.SpawnCompletedEvent += OnViewSpawnCompleted;
        }

        public void Handle(IFieldObjectHandler handler)
        {
            handler.Handle(this);
        }

        public void Handle(IFieldItemHandler handler)
        {
            handler.Handle(this);
        }

        public void Use(IUnit unit)
        {
            foreach (var orb in new List<ExpOrb>(_expOrbFinder.FindAll()))
                orb.MoveTo(unit, 10f);
            Destroy();
        }

        public Vector2 GetPosition() => _view.GetPosition();
        public void SetPosition(Vector2 position) => _view.SetPosition(position);

        private void OnViewSpawnCompleted(object sender, EventArgs e)
            => SpawnCompletedEvent?.Invoke(this, EventArgs.Empty);

        private void OnViewDestroyEvent(object sender, DestroyEventArgs e) => Destroy();

        public void Destroy()
        {
            if (_isDestroyed)
                return;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            _isDestroyed = true;

            _view.SpawnCompletedEvent -= OnViewSpawnCompleted;
            _view.DestroyEvent -= OnViewDestroyEvent;
            _view.Destroy();
        }
    }
}
