using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IWorld
    {
        event EventHandler PausedEvent;
        event EventHandler ResumedEvent;

        Vector2 GetPlayerUnitSpawnPosition();
        Vector2 GetWarpPoint();
        bool IsOutOfCameraView(Vector2 vector2);
        bool TryGetRandomPositionAround(Vector2 center, float minDistance, float maxDistance, out Vector2 position);
        void Pause();
        void Resume();
    }
}
