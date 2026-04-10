using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface ISkillEffect
    {
        event EventHandler DestroyEvent;
        void SetPosition(Vector2 position);
        void SetRotation(Vector2 direction);
    }
}