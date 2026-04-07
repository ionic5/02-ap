using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class ExpOrbFactory
    {
        public event EventHandler<CreatedEventArgs<ExpOrb>> ExpOrbCreatedEvent;

        private readonly Func<View.BattleFieldScene.IExpOrb> _createExpOrb;

        public ExpOrbFactory(Func<View.BattleFieldScene.IExpOrb> createExpOrb)
        {
            _createExpOrb = createExpOrb;
        }

        public ExpOrb Create(int level)
        {
            var view = _createExpOrb();
            var expOrb = new ExpOrb(view, level);
            ExpOrbCreatedEvent?.Invoke(this, new CreatedEventArgs<ExpOrb>(expOrb));
            return expOrb;
        }
    }
}
