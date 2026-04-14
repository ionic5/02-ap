using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IWorld
    {
        event EventHandler PausedEvent;
        event EventHandler ResumedEvent;

        Vector2 GetPlayerUnitSpawnPosition();
        Vector2 GetNextSpawnPoint();
        bool IsOutOfCameraView(Vector2 vector2);
        void Pause();
        void Resume();
    }
}
