using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class MeleeAttackSkill : ActiveSkill, ISkill
    {
        private readonly Core.Timer _cooldownTimer;
        private readonly Core.Timer _impactTimer;
        private readonly Core.Random _random;
        private UseSkillArgs _useSkillArgs;
        private State _state;
        private Vector2 _attackDirection;

        private enum State
        {
            Initial,
            Using,
            Completed,
            Canceled
        }

        public MeleeAttackSkill(Func<Timer> createTimer, Entity.ISkill skillEntity, Random random) : base(skillEntity)
        {
            _cooldownTimer = createTimer();
            _impactTimer = createTimer();
            _random = random;
            _state = State.Initial;
        }

        public override bool IsCooldownFinished()
        {
            return !_cooldownTimer.IsRunning();
        }

        public override void Use(UseSkillArgs args)
        {
            _state = State.Using;

            var user = GetOwner();
            var target = args.Target;
            _attackDirection = Vector2.Normalize(target.GetPosition() - user.GetPosition());

            _cooldownTimer.Start(GetAttribute(AttributeID.AttackTime).AsFloat(), OnCooldownFinished);
            _impactTimer.Start(GetAttribute(AttributeID.AttackImpactTime).AsFloat(), OnAttackImpact);

            SetUseSkillArgs(args);
        }

        private void SetUseSkillArgs(UseSkillArgs args)
        {
            _useSkillArgs = args;

            var user = GetOwner();
            user.DiedEvent += OnUserDiedEvent;
        }

        private void UnsetUseSkillArgs()
        {
            var user = GetOwner();
            user.DiedEvent -= OnUserDiedEvent;

            _useSkillArgs = default;
        }

        private void OnUserDiedEvent(object sender, DiedEventArgs e)
        {
            _state = State.Initial;

            _cooldownTimer.Stop();
            _impactTimer.Stop();

            UnsetUseSkillArgs();
        }

        private void OnCooldownFinished()
        {
            if (_state != State.Using)
                return;
            _state = State.Completed;

            var onCompleted = _useSkillArgs.OnCompleted;
            UnsetUseSkillArgs();

            onCompleted?.Invoke();
        }

        private void OnAttackImpact()
        {
            if (_state != State.Using)
                return;

            var user = GetOwner();
            var target = _useSkillArgs.Target;

            var targets = new HashSet<ITarget>();

            var attackRange = GetAttribute(AttributeID.AttackRange).AsFloat();
            var degree = GetAttribute(AttributeID.SwingAngle).AsFloat();
            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();

            if (target.IsAlive() && IsTargetInRange(user, target))
                targets.Add(target);

            if (degree > 0)
            {
                var position = user.GetPosition();
                var enemies = user.FindTargetsInSector(position, _attackDirection, degree, attackRange);

                targets.UnionWith(enemies);
            }

            foreach (var entry in targets)
            {
                var damage = _random.Next(minDmg, maxDmg);
                entry.Hit(user, damage);
            }
        }

        public override bool IsTargetInRange(IUnit unit, ITarget target)
        {
            var range = GetAttribute(AttributeID.AttackRange).AsFloat();
            var distSq = Vector2.DistanceSquared(unit.GetPosition(), target.GetPosition());
            return distSq <= range * range;
        }

        public override IEnumerable<ITarget> GetTargetsInRange(IUnit unit)
        {
            return unit.FindTargets(GetAttribute(AttributeID.AttackRange).AsFloat());
        }

        public override bool IsCompleted()
        {
            return _state == State.Completed;
        }

        public override void Cancel()
        {
            if (_state != State.Using)
                return;
            _state = State.Canceled;

            UnsetUseSkillArgs();

            if (_cooldownTimer.IsRunning() && _impactTimer.IsRunning())
                _cooldownTimer.Stop();
        }
    }
}
