using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class ExpOrbFactory
    {
        public event EventHandler<CreatedEventArgs<ExpOrb>> ExpOrbCreatedEvent;

        private readonly Func<string, View.BattleFieldScene.IExpOrb> _createExpOrb;
        private readonly GameDataStore _gameDataStore;

        public ExpOrbFactory(Func<string, View.BattleFieldScene.IExpOrb> createExpOrb, GameDataStore gameDataStore)
        {
            _createExpOrb = createExpOrb;
            _gameDataStore = gameDataStore;
        }

        public ExpOrb Create(string id)
        {
            var data = _gameDataStore.GetExpOrb(id);
            var view = _createExpOrb(data.BodyID);
            var expOrb = new ExpOrb(view, data.Exp);
            ExpOrbCreatedEvent?.Invoke(this, new CreatedEventArgs<ExpOrb>(expOrb));
            return expOrb;
        }
    }
}
