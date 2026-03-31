using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IMortal : IPositionable
    {
        event EventHandler<DiedEventArgs> DiedEvent;
        bool IsDead();
    }
}
