using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public class BatchObjectDetectedEventArgs
    {
        public readonly IEnumerable<string> ObjectIDs;

        public BatchObjectDetectedEventArgs(IEnumerable<string> objectIDs)
        {
            ObjectIDs = objectIDs;
        }
    }
}
