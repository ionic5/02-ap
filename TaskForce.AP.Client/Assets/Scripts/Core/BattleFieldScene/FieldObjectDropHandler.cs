using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class FieldObjectDropHandler
    {
        private readonly ExpOrbFactory _expOrbFactory;
        private readonly FieldItemFactory _fieldItemFactory;
        private readonly Core.Random _random;
        private readonly GameDataStore _gameDataStore;

        public FieldObjectDropHandler(ExpOrbFactory expOrbFactory, FieldItemFactory fieldItemFactory,
            Core.Random random, GameDataStore gameDataStore)
        {
            _expOrbFactory = expOrbFactory;
            _fieldItemFactory = fieldItemFactory;
            _random = random;
            _gameDataStore = gameDataStore;
        }

        public void OnEnemyKilled(object sender, DiedEventArgs args)
        {
            if (args.Killer == null || !args.Killer.IsPlayerSide())
                return;

            var dropRate = _gameDataStore.GetSoulDropRate();
            if (_random.Next(0.0f, 100.0f) >= dropRate)
                return;

            var expOrb = _expOrbFactory.Create(1);
            expOrb.SetPosition(args.DiedTarget.GetPosition());
        }

        public void OnAllBossesKilled(object sender, DiedEventArgs args)
        {
            var expOrb = _expOrbFactory.Create(1);
            expOrb.SetPosition(args.DiedTarget.GetPosition());
        }

        public void OnRootBoxDied(object sender, DiedEventArgs args)
        {
            var roll = _random.Next(0, 3);
            var itemId = roll == 0
                ? GameData.FieldItemID.Nuke
                : roll == 1
                    ? GameData.FieldItemID.MedicalKit
                    : GameData.FieldItemID.GoldBundle;

            IFieldItem item = _fieldItemFactory.Create(itemId);
            item.SetPosition(args.DiedTarget.GetPosition());
        }
    }
}
