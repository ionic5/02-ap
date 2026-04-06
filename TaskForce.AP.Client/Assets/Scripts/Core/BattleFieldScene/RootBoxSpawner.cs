using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class RootBoxSpawner
    {
        private readonly RootBoxFactory _factory;
        private readonly Core.Timer _spawnTimer;
        private readonly Core.Timer _repositionTimer;
        private readonly View.BattleFieldScene.IWorld _world;
        private readonly IUnit _player;
        private readonly GameDataStore _gameDataStore;

        private readonly List<RootBox> _activeRootBoxes = new List<RootBox>();
        private bool _isDestroyed;

        public RootBoxSpawner(RootBoxFactory factory, Core.Timer spawnTimer, Core.Timer repositionTimer,
            View.BattleFieldScene.IWorld world, GameDataStore gameDataStore, IUnit player)
        {
            _factory = factory;
            _spawnTimer = spawnTimer;
            _repositionTimer = repositionTimer;
            _world = world;
            _gameDataStore = gameDataStore;
            _player = player;
        }

        public void Start()
        {
            if (_activeRootBoxes.Count < GetMaxCount())
                _spawnTimer.Start(1f, OnSpawnTimerElapsed);
            _repositionTimer.Start(GetRepositionInterval(), OnRepositionTimerElapsed);
        }

        private int GetMaxCount()             => _gameDataStore.GetConstant(GameData.ConstantID.RootBoxMaxCount).AsInt();
        private float GetMinDistance()        => _gameDataStore.GetConstant(GameData.ConstantID.RootBoxMinDistance).AsFloat();
        private float GetMaxDistance()        => _gameDataStore.GetConstant(GameData.ConstantID.RootBoxMaxDistance).AsFloat();
        private float GetRepositionInterval() => _gameDataStore.GetConstant(GameData.ConstantID.RootBoxRepositionInterval).AsFloat();

        private void OnSpawnTimerElapsed()
        {
            if (!_world.TryGetRandomPositionAround(_player.GetPosition(), GetMinDistance(), GetMaxDistance(), out var position))
            {
                if (_activeRootBoxes.Count < GetMaxCount())
                    _spawnTimer.Start(1f, OnSpawnTimerElapsed);
                return;
            }

            var rootBox = _factory.Create();
            rootBox.SetPosition(position);
            rootBox.DestroyEvent += OnRootBoxDestroyed;
            _activeRootBoxes.Add(rootBox);

            if (_activeRootBoxes.Count < GetMaxCount())
                _spawnTimer.Start(1f, OnSpawnTimerElapsed);
        }

        private void OnRepositionTimerElapsed()
        {
            _repositionTimer.Start(GetRepositionInterval(), OnRepositionTimerElapsed);

            if (_activeRootBoxes.Count == 0)
                return;

            var playerPos = _player.GetPosition();
            RootBox farthest = null;
            float maxDistSq = -1f;
            foreach (var rootBox in _activeRootBoxes)
            {
                var diff = rootBox.GetPosition() - playerPos;
                var distSq = diff.X * diff.X + diff.Y * diff.Y;
                if (distSq > maxDistSq)
                {
                    maxDistSq = distSq;
                    farthest = rootBox;
                }
            }

            if (!_world.IsOutOfCameraView(farthest.GetPosition()))
                return;

            if (_world.TryGetRandomPositionAround(playerPos, GetMinDistance(), GetMaxDistance(), out var position))
                farthest.SetPosition(position);
        }

        private void OnRootBoxDestroyed(object sender, DestroyEventArgs e)
        {
            var rootBox = (RootBox)sender;
            rootBox.DestroyEvent -= OnRootBoxDestroyed;
            _activeRootBoxes.Remove(rootBox);

            if (!_isDestroyed && _activeRootBoxes.Count < GetMaxCount())
                _spawnTimer.Start(1f, OnSpawnTimerElapsed);
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;
            _isDestroyed = true;
            _spawnTimer.Stop();
            _repositionTimer.Stop();
        }
    }
}
