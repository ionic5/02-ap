using System;
using System.Collections.Generic;
using TaskForce.AP.Client.Core.Entity;
using Vector2 = System.Numerics.Vector2;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public class MeleeDaggerSkill : ActiveSkill, ISkill
    {
        private readonly Core.Timer _cooldownTimer;
        private readonly Core.Timer _impactTimer;
        private readonly Core.Timer _comboTimer;
        private readonly Core.Random _random;
        private UseSkillArgs _useSkillArgs;
        private State _state;
        private ILogger _logger;

        private int _attackComboCount;
        private readonly Func<IUnit, SkillEffectDagger> _createSkillEffectDagger;

        private enum State
        {
            Initial,
            Using,
            Completed,
            Canceled
        }

        public MeleeDaggerSkill(Func<Timer> createTimer, Entity.ISkill skillEntity, Random random, ILogger logger, Func<IUnit, SkillEffectDagger> createSkillEffectDagger) : base(skillEntity)
        {
            _cooldownTimer = createTimer();
            _impactTimer = createTimer();
            _comboTimer = createTimer();
            _random = random;
            _state = State.Initial;
            _logger = logger;
            _createSkillEffectDagger = createSkillEffectDagger;
        }

        public override bool IsCooldownFinished()
        {
            return !_cooldownTimer.IsRunning();
        }

        public override void Use(UseSkillArgs args)
        {
            _state = State.Using;

            _attackComboCount = GetAttribute(AttributeID.AttackCombo).AsInt();
            
            StartAttackCombo(args);
            _cooldownTimer.Start(GetAttribute(AttributeID.AttackTime).AsFloat(), OnCooldownFinished);
            
            SetUseSkillArgs(args);
        }

        void StartAttackCombo(UseSkillArgs args)
        {
            _attackComboCount--;
            
            var user = GetOwner();
            var target = args.Target;
            var attackTime = GetAttribute(AttributeID.AttackTime).AsFloat();

            var attackDirection = Vector2.Normalize(target.GetPosition() - user.GetPosition());
            user.Attack(attackDirection, attackTime);
            _impactTimer.Start(GetAttribute(AttributeID.AttackImpactTime).AsFloat(), OnAttackImpact);

            var skillEffect = _createSkillEffectDagger.Invoke(user);
            skillEffect.SetFollow(user);
            skillEffect.SetRotation(user.GetDirection());
            
            if (_attackComboCount > 0)
            {  
                _comboTimer.Start(GetAttribute(AttributeID.AttackComboTime).AsFloat(), () => StartAttackCombo(args));
            }
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
            var target = _useSkillArgs.Target;

            var targets = new HashSet<ITarget>();

            var attackRange = GetAttribute(AttributeID.AttackRange).AsFloat();
            var degree = GetAttribute(AttributeID.AttackAngle).AsFloat();  
            var minDmg = GetAttribute(AttributeID.MinDamage).AsInt();
            var maxDmg = GetAttribute(AttributeID.MaxDamage).AsInt();
            
            var attackTime = GetAttribute(AttributeID.AttackTime).AsFloat();
            var attackImpactTime = GetAttribute(AttributeID.AttackImpactTime).AsFloat();
            var attackCombo = GetAttribute(AttributeID.AttackCombo).AsInt();
            var attackComboTime = GetAttribute(AttributeID.AttackComboTime).AsFloat();
            
            // TODO: JW: test용 주석 삭제 요
            // _logger.Info($"meleeDagger: OnAttackImpact: attackTime: {attackTime}, attackImpactTime:{attackImpactTime}," +
            //              $"attackRange: {attackRange}, degree: {degree}, minDamage: {minDmg}, maxDamage: {maxDmg}, " +
            //              $"attackCombo: {attackCombo}, attackComboTime: {attackComboTime}");
            
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

            if (_cooldownTimer.IsRunning() && _impactTimer.IsRunning())
                _cooldownTimer.Stop();
        }
    }
}
