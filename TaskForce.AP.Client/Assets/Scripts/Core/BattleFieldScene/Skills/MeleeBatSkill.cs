using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using log4net.Appender;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class MeleeBatSkill : InstantSkill, ISkill
    {
        private readonly Core.Timer _cooldownTimer;
        private readonly Core.Timer _impactTimer;
        private readonly Core.Timer _comboTimer;
        private readonly Core.Random _random;
        private UseSkillArgs _useSkillArgs;
        private State _state;
        private ILogger _logger;
        private readonly Func<IUnit, SkillEffectMelee> _createSkillEffectBat;

        private enum State
        {
            Initial,
            Using,
            Completed,
            Canceled
        }

        public MeleeBatSkill(Func<Timer> createTimer, Entity.ISkill skillEntity, Random random, ILogger logger, Func<IUnit, SkillEffectMelee> createSkillEffectBat) : base(skillEntity)
        {
            _cooldownTimer = createTimer();
            _impactTimer = createTimer();
            _comboTimer = createTimer();
            _random = random;
            _state = State.Initial;
            _logger = logger;
            _createSkillEffectBat = createSkillEffectBat;
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
            var attackTime = GetAttribute(AttributeID.AttackTime).AsFloat();
            
            user.Attack(user.GetDirection(), attackTime);
            _impactTimer.Start(GetAttribute(AttributeID.AttackImpactTime).AsFloat(), OnAttackImpact);
            _cooldownTimer.Start(GetAttribute(AttributeID.AttackTime).AsFloat(), OnCooldownFinished);
            
            var skillEffect = _createSkillEffectBat?.Invoke(user);
            skillEffect.SetFollow(user);
            skillEffect.SetRotation(user.GetDirection());
            
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
            _comboTimer.Stop();

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

            var targets = new HashSet<ITarget>();

            var attackRange = GetAttribute(AttributeID.AttackRange).AsFloat();
            var degree = GetAttribute(AttributeID.AttackAngle).AsFloat();  
            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();
            
            var attackTime = GetAttribute(AttributeID.AttackTime).AsFloat();
            var attackImpactTime = GetAttribute(AttributeID.AttackImpactTime).AsFloat();
            var attackCombo = GetAttribute(AttributeID.AttackCombo).AsInt();
            var attackComboTime = GetAttribute(AttributeID.AttackComboTime).AsFloat();

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

            if (_cooldownTimer.IsRunning() && _impactTimer.IsRunning())
                _cooldownTimer.Stop();
        }
    }
}
