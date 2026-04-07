using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IFieldItem : IFieldObject
    {
        event EventHandler SpawnCompletedEvent;
        void Handle(IFieldItemHandler handler);
    }
}
