using System.Collections.Generic;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IExpOrbFinder
    {
        IEnumerable<ExpOrb> FindAll();
    }
}
