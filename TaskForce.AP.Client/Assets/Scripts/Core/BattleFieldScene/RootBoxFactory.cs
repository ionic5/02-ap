using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class RootBoxFactory
    {
        public event EventHandler<CreatedEventArgs<RootBox>> RootBoxCreatedEvent;

        private readonly Func<View.BattleFieldScene.IRootBox> _createRootBox;
        private readonly GameDataStore _gameDataStore;

        public RootBoxFactory(Func<View.BattleFieldScene.IRootBox> createRootBox, GameDataStore gameDataStore)
        {
            _createRootBox = createRootBox;
            _gameDataStore = gameDataStore;
        }

        public RootBox Create()
        {
            var hp = _gameDataStore.GetConstant(GameData.ConstantID.RootBoxHp).AsInt();
            var view = _createRootBox();
            var rootBox = new RootBox(view, hp);
            RootBoxCreatedEvent?.Invoke(this, new CreatedEventArgs<RootBox>(rootBox));
            return rootBox;
        }
    }
}
