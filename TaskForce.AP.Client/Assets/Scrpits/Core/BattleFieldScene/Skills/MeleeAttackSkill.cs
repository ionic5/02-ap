using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class MeleeAttackSkill : ActiveSkill, ISkill
    {
        private readonly Core.Timer _timer;
        private readonly Core.Random _random;
        private UseSkillArgs _useSkillArgs;
        private State _state;

        private enum State
        {
            Initial,
            Using,
            Completed,
            Canceled
        }

        private static class TimerType
        {
            public const int Cooldown = 0;
            public const int Impact = 1;
        }

        public MeleeAttackSkill(Timer timer, Entity.ISkill skillEntity, Random random) : base(skillEntity)
        {
            _timer = timer;
            _random = random;
            _state = State.Initial;
        }

        public override bool IsCooldownFinished()
        {
            return !_timer.IsRunning(TimerType.Cooldown);
        }

        public override void Use(UseSkillArgs args)
        {
            _state = State.Using;

            var user = GetOwner();
            var target = args.Target;
            var attackTime = GetAttribute(AttributeID.AttackTime).AsFloat();

            var attackDirection = Vector2.Normalize(target.GetPosition() - user.GetPosition());
            user.Attack(attackDirection, attackTime);
            _timer.Start(TimerType.Cooldown, GetAttribute(AttributeID.AttackTime).AsFloat(), OnCooldownFinished);
            _timer.Start(TimerType.Impact, GetAttribute(AttributeID.AttackImpactTime).AsFloat(), OnAttackImpact);

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

            _timer.Stop();

            UnsetUseSkillArgs();
        }

        private void OnCooldownFinished()
        {
            if (_state != State.Using)
                return;
            _state = State.Completed;

            var user = GetOwner();
            var onCompleted = _useSkillArgs.OnCompleted;
            UnsetUseSkillArgs();

            user.Wait();
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
                var direction = user.GetDirection();
                var enemies = user.FindTargetsInSector(position, direction, degree, attackRange);

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

            var user = GetOwner();
            user.Wait();

            UnsetUseSkillArgs();

            if (_timer.IsRunning(TimerType.Cooldown) && _timer.IsRunning(TimerType.Impact))
                _timer.Stop(TimerType.Cooldown);
        }
    }
}
