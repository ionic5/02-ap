using System;
using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface ITargetFinder
    {
        ITarget FindByViewID(string objectID);
        IEnumerable<ITarget> FindByViewIDs(IEnumerable<string> objectIDs, Func<ITarget, bool> predicate);
        IEnumerable<ITarget> FindInRadius(System.Numerics.Vector2 center, float minRadius, float maxRadius, Func<ITarget, bool> predicate);
        IEnumerable<ITarget> FindInRadius(System.Numerics.Vector2 center, float radius, Func<ITarget, bool> predicate);
        IEnumerable<ITarget> FindInSector(System.Numerics.Vector2 center, System.Numerics.Vector2 direction, float degree, float radius, Func<ITarget, bool> predicate);
    }
}
