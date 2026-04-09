using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class MedicalKit : IFieldItem
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;
        public event EventHandler SpawnCompletedEvent;

        private bool _isDestroyed;
        private readonly View.BattleFieldScene.IFieldItem _view;
        private readonly GameDataStore _gameDataStore;

        public MedicalKit(View.BattleFieldScene.IFieldItem view, GameDataStore gameDataStore)
        {
            _view = view;
            _gameDataStore = gameDataStore;
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
            float rate = _gameDataStore.GetConstant(GameData.ConstantID.MedicalKitHealRate).AsFloat();
            int healAmount = (int)(unit.GetAttribute(Entity.AttributeID.MaxHP).AsInt() * rate);
            unit.Heal(healAmount);
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
