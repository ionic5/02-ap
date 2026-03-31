using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class CreatedEventArgs<T> : EventArgs
    {
        public readonly T CreatedObject;

        public CreatedEventArgs(T createdObject)
        {
            CreatedObject = createdObject;
        }
    }
}
