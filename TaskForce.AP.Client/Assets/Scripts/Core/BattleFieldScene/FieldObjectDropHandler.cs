using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class FieldObjectDropHandler
    {
        private readonly ExpOrbFactory _expOrbFactory;
        private readonly FieldItemFactory _fieldItemFactory;
        private readonly Core.Random _random;

        public FieldObjectDropHandler(ExpOrbFactory expOrbFactory, FieldItemFactory fieldItemFactory,
            Core.Random random)
        {
            _expOrbFactory = expOrbFactory;
            _fieldItemFactory = fieldItemFactory;
            _random = random;
        }

        public void OnEnemyKilled(object sender, DiedEventArgs args)
        {
            if (args.Killer == null || !args.Killer.IsPlayerSide())
                return;

            var expOrb = _expOrbFactory.Create(GameData.ExpOrbID.ExpOrb0);
            expOrb.SetPosition(args.DiedTarget.GetPosition());
        }

        public void OnAllBossesKilled(object sender, DiedEventArgs args)
        {
            var expOrb = _expOrbFactory.Create(GameData.ExpOrbID.ExpOrb0);
            expOrb.SetPosition(args.DiedTarget.GetPosition());
        }

        public void OnRootBoxDied(object sender, DiedEventArgs args)
        {
            string[] itemIds = { GameData.FieldItemID.Nuke, GameData.FieldItemID.Magnet, GameData.FieldItemID.MedicalKit, GameData.FieldItemID.GoldBundle };
            var itemId = itemIds[_random.Next(itemIds.Length)];

            IFieldItem item = _fieldItemFactory.Create(itemId);
            item.SetPosition(args.DiedTarget.GetPosition());
        }
    }
}
