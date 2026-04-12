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

            var unitID = args.DiedTarget.GetUnitID();
            var rewardExpOrb = _gameDataStore.GetStageRewardExpOrbByUnitID(unitID);
            if (rewardExpOrb == null)
                return;

            var expOrb = _expOrbFactory.Create(rewardExpOrb.ExpOrbID);
            expOrb.SetPosition(args.DiedTarget.GetPosition());
        }

        public void OnBossStageCleared(object sender, BossStageClearedEventArgs args)
        {
            var rewards = _gameDataStore.GetBossStageRewardExpOrbs(args.BossStageLevel);
            foreach (var reward in rewards)
            {
                for (var i = 0; i < reward.Count; i++)
                {
                    var expOrb = _expOrbFactory.Create(reward.ExpOrbID);
                    expOrb.SetPosition(args.LastBossDiedPosition);
                }
            }
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
