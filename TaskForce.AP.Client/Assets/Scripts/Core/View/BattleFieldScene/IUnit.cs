using System;
using System.Drawing;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IUnit : IDestroyable
    {
        event EventHandler DieAnimationFinishedEvent;
        event EventHandler MoveDirectionChangedEvent;
        event EventHandler DeathAnimationCompletedEvent; // Add new event
        void Die(); // Add new method
        void SetActive(bool active); // Add new method
        void SetHpRatio(float ratio);
        void SetHPBarVisible(bool visible);
        void PlayReviveEffect(Action onCompleted); // [REVIVE_EFFECT_TEST]

        string GetObjectID();
        void PlayMotion(UnitMotionID motionID, Vector2 direction, float playTime, bool forceRestart);
        void PlayMotion(UnitMotionID motionID);
        void PlayDamageAnimation(int damage);
        void PlayHealAnimation(int healAmount);
        void SetPosition(Vector2 position);
        Vector2 GetPosition();
        void SetDirection(Vector2 vector2);
        Vector2 GetDirection();
        void Move(Vector2 velocity);
        void MoveTo(Vector2 position, float speed);
        void Stop();
        Vector2 GetMoveDirection();
        void SetHitTextColor(Color color);
    }
}
