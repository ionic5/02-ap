using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class UnitLogicFactory
    {
        private readonly IJoystick _joystick;
        private readonly IFieldObjectFinder _fieldObjectFinder;
        private readonly Core.View.BattleFieldScene.IWorld _world;
        private readonly Dictionary<string, Func<IControllableUnit, IUnitLogic>> _creationFunction;
        private readonly UserDataStore _userDataStore;
        private readonly ILoop _loop;
        private readonly ILogger _logger;
        private readonly Func<Core.Timer> _createTimer;

        public UnitLogicFactory(IJoystick joystick, View.BattleFieldScene.IWorld world,
            Func<Timer> createTimer, ILoop loop, IFieldObjectFinder fieldObjectFinder, ILogger logger, UserDataStore userDataStore)
        {
            _joystick = joystick;
            _world = world;
            _createTimer = createTimer;
            _loop = loop;
            _fieldObjectFinder = fieldObjectFinder;
            _userDataStore = userDataStore;
            _logger = logger;

            _creationFunction = new Dictionary<string, Func<IControllableUnit, IUnitLogic>>{
                { "PLAYER", (unit) =>  new PlayerUnitLogic(_loop, _joystick, _fieldObjectFinder, _userDataStore) },
                { "NON_PLAYER", (unit) =>  new NonPlayerUnitLogic(_loop, _createTimer.Invoke(), _world) },
                { "MONK", (unit) =>  new MonkLogic(_loop, new Core.Random()) }
            };
        }

        public IUnitLogic Create(IControllableUnit unit, string logicID)
        {
            if (_creationFunction.TryGetValue(logicID, out var creationFunction))
                return creationFunction.Invoke(unit);

            _logger.Warn($"Failed to find logic for logic id ({logicID})");
            return null;
        }
    }
}
