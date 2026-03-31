using System.Collections.Generic;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface ISoulFinder
    {
        int FindRadius(Vector2 center, float radius, List<Soul> results);
    }
}
