using System;
using System.Numerics;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class GoldBundle : IFieldItem
    {
        public event EventHandler<DestroyEventArgs> DestroyEvent;
        public event EventHandler SpawnCompletedEvent;

        private bool _isDestroyed;
        private readonly View.BattleFieldScene.IFieldItem _view;
        private readonly StageHost _stageHost;
        private readonly GameDataStore _gameDataStore;
        private readonly Random _random;

        public GoldBundle(View.BattleFieldScene.IFieldItem view, StageHost stageHost, GameDataStore gameDataStore, Random random)
        {
            _view = view;
            _stageHost = stageHost;
            _gameDataStore = gameDataStore;
            _random = random;
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

        public void Use(UserDataStore userDataStore)
        {
            var reward = _gameDataStore.GetGoldBundleReward(_stageHost.GetStageLevel());
            var gold = reward != null ? _random.Next(reward.MinGold, reward.MaxGold + 1) : 0;
            userDataStore.AddGold(gold);
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
