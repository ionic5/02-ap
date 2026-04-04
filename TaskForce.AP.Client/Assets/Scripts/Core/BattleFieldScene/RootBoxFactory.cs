using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class RootBoxFactory
    {
        public event EventHandler<CreatedEventArgs<RootBox>> RootBoxCreatedEvent;

        private readonly Func<View.BattleFieldScene.IRootBox> _createRootBox;

        public RootBoxFactory(Func<View.BattleFieldScene.IRootBox> createRootBox)
        {
            _createRootBox = createRootBox;
        }

        public RootBox Create(int hp, Action onDied)
        {
            var view = _createRootBox();
            var rootBox = new RootBox(view, hp, onDied);
            RootBoxCreatedEvent?.Invoke(this, new CreatedEventArgs<RootBox>(rootBox));
            return rootBox;
        }
    }
}
