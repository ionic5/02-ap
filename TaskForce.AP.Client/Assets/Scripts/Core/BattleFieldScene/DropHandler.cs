using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class DropHandler
    {
        private readonly ExpOrbFactory _expOrbFactory;
        private readonly Core.Random _random;
        private readonly GameDataStore _gameDataStore;

        public DropHandler(ExpOrbFactory expOrbFactory, Random random, GameDataStore gameDataStore)
        {
            _expOrbFactory = expOrbFactory;
            _random = random;
            _gameDataStore = gameDataStore;
        }

        public void OnUnitCreatedEvent(object sender, CreatedEventArgs<Unit> args)
        {
            var newUnit = args.CreatedObject;

            EventHandler<DiedEventArgs> hdlr = null;
            hdlr = (sender, args) =>
            {
                OnUnitDiedEvent(sender, args);
                newUnit.DiedEvent -= hdlr;
            };
            newUnit.DiedEvent += hdlr;
        }

        public void OnUnitDiedEvent(object sender, DiedEventArgs args)
        {
            var dropRate = _gameDataStore.GetSoulDropRate();
            if (_random.Next(0.0f, 100.0f) >= dropRate)
                return;

            var expOrb = _expOrbFactory.Create(1);
            expOrb.SetPosition(args.DiedTarget.GetPosition());
        }
    }
}
