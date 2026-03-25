using System;
using System.Collections.Generic;
using System.Numerics;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IUnit : IDestroyable, IMortal, IPositionable, IFollowable
    {
        event EventHandler ExpChangedEvent;
        event EventHandler RequireExpChangedEvent;
        event EventHandler LevelUpEvent;
        event EventHandler DeathAnimationCompletedEvent; // Add new event
        void Die(); // Add new method
        void SetActive(bool active);
        void SetInvincible(bool isInvincible);
        void PlayReviveEffect(Action onCompleted); // [REVIVE_EFFECT_TEST]

        Attribute GetAttribute(string attributeID);
        int GetExp();
        int GetRequireExp();
        int GetLevel();
        Vector2 GetDirection();
        Skills.ISkill GetDefaultSkill();
        bool IsPlayerSide();
        bool IsEnemy(ITarget target);
        IEnumerable<ITarget> FindAllies(float range);
        IEnumerable<ITarget> FindTargets(float range);
        IEnumerable<ITarget> FindTargets(float minRange, float maxRange);
        IEnumerable<ITarget> FindTargets(Vector2 center, float range);
        IEnumerable<ITarget> FindTargetsInSector(Vector2 center, Vector2 direction, float angle, float range);
        IEnumerable<ITarget> FindTargets(IEnumerable<string> objectIDs);
        void Cast(float castTime);
        void Attack(Vector2 direction, float attackTime);
        void Wait();
        void SetUnitLogic(string logicID);
        void SetMaster(IUnit summoner);
        void RecoverFullHP();
    }
}
