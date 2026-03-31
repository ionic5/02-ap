using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class DropHandler
    {
        private readonly SoulFactory _soulFactory;
        private readonly Core.Random _random;
        private readonly GameDataStore _gameDataStore;

        public DropHandler(SoulFactory soulFactory, Random random, GameDataStore gameDataStore)
        {
            _soulFactory = soulFactory;
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

            var soul = _soulFactory.Create(1);
            soul.SetPosition(args.DiedTarget.GetPosition());
        }
    }
}
