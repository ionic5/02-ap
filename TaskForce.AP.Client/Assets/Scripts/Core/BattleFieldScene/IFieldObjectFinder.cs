using System.Collections.Generic;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IFieldObjectFinder
    {
        int FindRadius(Vector2 center, float radius, List<IFieldObject> results);
    }
}
