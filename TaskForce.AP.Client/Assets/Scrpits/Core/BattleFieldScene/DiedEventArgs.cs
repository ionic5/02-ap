using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class DiedEventArgs : EventArgs
    {
        public readonly IMortal DiedTarget;
        public readonly IUnit Killer;

        public DiedEventArgs(IMortal diedTarget, IUnit killer = null)
        {
            DiedTarget = diedTarget;
            Killer = killer;
        }
    }
}
