using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public class BatchObjectHitEventArgs : EventArgs
    {
        public readonly IEnumerable<string> ObjectIDs;

        public BatchObjectHitEventArgs(IEnumerable<string> objectIDs)
        {
            ObjectIDs = objectIDs;
        }
    }
}
