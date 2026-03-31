using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IOneShotEffect
    {
        void Follow(IFollowable target);
        void Play();
        void SetPosition(Vector2 vector2);
    }
}
