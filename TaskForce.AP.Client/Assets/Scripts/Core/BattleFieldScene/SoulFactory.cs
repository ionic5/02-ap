using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class SoulFactory
    {
        public event EventHandler<CreatedEventArgs<Soul>> SoulCreatedEvent;

        private readonly Func<View.BattleFieldScene.ISoul> _createSoul;

        public SoulFactory(Func<View.BattleFieldScene.ISoul> createSoul)
        {
            _createSoul = createSoul;
        }

        public Soul Create(int power)
        {
            var view = _createSoul();

            var soul = new Soul(view, power);

            SoulCreatedEvent.Invoke(this, new CreatedEventArgs<Soul>(soul));

            return soul;
        }
    }
}
