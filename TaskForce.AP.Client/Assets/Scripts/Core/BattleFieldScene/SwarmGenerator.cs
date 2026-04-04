using System;
using System.Linq;
using TaskForce.AP.Client.Core.GameData;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class SwarmGenerator
    {
        private readonly View.BattleFieldScene.IWorld _world;
        private readonly GameDataStore _gameDataStore;
        private readonly Core.Timer _swarmTimer;
        private readonly Action<string, int, System.Numerics.Vector2> _createUnit;

        private EnemyUnitSwarm _swarmData;
        private float _swarmInterval;

        public SwarmGenerator(View.BattleFieldScene.IWorld world, GameDataStore gameDataStore,
            Timer swarmTimer, Action<string, int, System.Numerics.Vector2> createUnit)
        {
            _world = world;
            _gameDataStore = gameDataStore;
            _swarmTimer = swarmTimer;
            _createUnit = createUnit;
        }

        public void Start()
        {
            _swarmData = _gameDataStore.GetEnemyUnitSwarms().FirstOrDefault();
            _swarmInterval = _gameDataStore.GetConstant(GameData.ConstantID.EnemyUnitSwarmInterval).AsFloat();
            _swarmTimer.Start(_swarmInterval, OnSwarmTimerElapsed);
        }

        private void OnSwarmTimerElapsed()
        {
            SpawnSwarm();
            _swarmTimer.Start(_swarmInterval, OnSwarmTimerElapsed);
        }

        private void SpawnSwarm()
        {
            var spawnPos = _world.GetWarpPoint();
            for (var i = 0; i < _swarmData.Count; i++)
                _createUnit(_swarmData.UnitID, _swarmData.Level, spawnPos);
        }
    }
}
