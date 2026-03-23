using System;
using System.Collections.Generic;
using System.Numerics;
using TaskForce.AP.Client.Core.Entity;
using TaskForce.AP.Client.Core.View.BattleFieldScene;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class Unit : ITarget, IUnit, IAttacker, IDestroyable, IFollowable, IMortal,
        IPositionable, IControllableUnit
    {
        public event EventHandler ExpChangedEvent;
        public event EventHandler RequireExpChangedEvent;
        public event EventHandler LevelUpEvent;
        public event EventHandler<DestroyEventArgs> DestroyEvent;
        public event EventHandler<DiedEventArgs> DiedEvent;

        private readonly Core.View.BattleFieldScene.IUnit _unitView;
        private readonly Entity.Unit _unitEntity;
        private readonly ITargetIdentifier _targetIdentifier;
        private readonly Core.ILogger _logger;
        private readonly Dictionary<string, Skills.ISkill> _skills;
        private readonly ITargetFinder _targetFinder;
        private readonly Func<Entity.ISkill, Skills.ISkill> _createSkill;
        private readonly Func<IControllableUnit, string, IUnitLogic> _createUnitLogic;

        private bool _isDestroyed;
        private IUnit _master;
        private IUnitLogic _unitLogic;

        public Unit(Core.View.BattleFieldScene.IUnit unitView, Entity.Unit unitEntity,
            ITargetFinder targetFinder, ITargetIdentifier targetIdentifier,
            ILogger logger, Func<Core.Entity.ISkill, Skills.ISkill> createSkill,
            Func<IControllableUnit, string, IUnitLogic> createUnitLogic)
        {
            _unitView = unitView;
            _unitEntity = unitEntity;
            _isDestroyed = false;
            _skills = new Dictionary<string, Skills.ISkill>();
            _targetFinder = targetFinder;

            _unitEntity.ExpChangedEvent += OnExpChangedEvent;
            _unitEntity.RequireExpChangedEvent += OnRequireExpChangedEvent;
            _unitEntity.LevelUpEvent += OnLevelUpEvent;
            _unitEntity.SkillAddedEvent += OnSkillAddedEvent;

            _unitView.MoveDirectionChangedEvent += OnMoveDirectionChangedEvent;
            _unitView.DieAnimationFinishedEvent += OnDieAnimationFinishedEvent;
            _unitView.DestroyEvent += OnDestroyEvent;

            _targetIdentifier = targetIdentifier;
            _logger = logger;
            _createSkill = createSkill;
            _createUnitLogic = createUnitLogic;
        }

        public bool IsDead()
        {
            return _unitEntity.IsDead();
        }

        public void Wait()
        {
            if (IsDead())
                return;

            _unitView.Stop();
            _unitView.PlayMotion(UnitMotionID.Stand);
        }

        public void Move(Vector2 direction)
        {
            if (IsDead())
                return;

            _unitView.PlayMotion(UnitMotionID.Move);
            _unitView.Move(direction * _unitEntity.GetAttribute(AttributeID.MoveSpeed).AsFloat());
            _unitView.SetDirection(_unitView.GetMoveDirection());
        }

        public void MoveTo(Vector2 destination)
        {
            if (IsDead())
                return;

            _unitView.PlayMotion(UnitMotionID.Move);
            _unitView.MoveTo(destination, _unitEntity.GetAttribute(AttributeID.MoveSpeed).AsFloat());
            _unitView.SetDirection(_unitView.GetMoveDirection());
        }

        private void OnMoveDirectionChangedEvent(object sender, EventArgs args)
        {
            _unitView.SetDirection(_unitView.GetMoveDirection());
        }

        private void OnDieAnimationFinishedEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void OnDestroyEvent(object sender, DestroyEventArgs e)
        {
            Destroy();
        }

        public void Die()
        {
            if (IsDead())
                return;

            _unitEntity.SetDead();

            OnDie();
        }

        private void OnDie()
        {
            _unitView.Stop();
            _unitView.PlayMotion(UnitMotionID.Die);

            DiedEvent?.Invoke(this, new DiedEventArgs(this));
        }

        public void Hit(int damage)
        {
            if (IsDead())
                return;

            _unitEntity.ApplyDamage(damage);

            _unitView.PlayDamageAnimation(damage);

            if (IsDead())
                OnDie();
        }

        public Vector2 GetDirection()
        {
            return _unitView.GetDirection();
        }

        public Vector2 GetPosition()
        {
            return _unitView.GetPosition();
        }

        public bool IsAlive()
        {
            return _unitEntity.IsAlive();
        }

        public bool IsTargetInAttackRange(ITarget target)
        {
            Vector2 pos = _unitView.GetPosition();
            Vector2 targetPos = target.GetPosition();

            return Vector2.Distance(pos, targetPos) <= _unitEntity.GetAttribute(AttributeID.AttackRange).AsFloat();
        }

        public void Destroy()
        {
            if (_isDestroyed)
                return;

            DestroyEvent?.Invoke(this, new DestroyEventArgs(this));
            DestroyEvent = null;

            _isDestroyed = true;

            _unitEntity.ExpChangedEvent -= OnExpChangedEvent;
            _unitEntity.RequireExpChangedEvent -= OnRequireExpChangedEvent;
            _unitEntity.LevelUpEvent -= OnLevelUpEvent;
            _unitEntity.SkillAddedEvent -= OnSkillAddedEvent;

            _unitView.MoveDirectionChangedEvent -= OnMoveDirectionChangedEvent;
            _unitView.DieAnimationFinishedEvent -= OnDieAnimationFinishedEvent;
            _unitView.DestroyEvent -= OnDestroyEvent;

            _unitView.Destroy();

            DestroyUnitLogic();
        }

        public void SetPosition(Vector2 position)
        {
            _unitView.SetPosition(position);
        }

        public bool IsPlayerSide()
        {
            return _unitEntity.IsPlayerSide();
        }

        public Attribute GetAttribute(string id)
        {
            return _unitEntity.GetAttribute(id);
        }

        public bool IsEnemy(ITarget target)
        {
            if (target == this)
                return false;
            return _targetIdentifier.IsEnemy(target);
        }

        public float GetPickUpRange()
        {
            return 0.8f;
        }

        public void AddExp(int exp)
        {
            _unitEntity.AddExp(exp);
        }

        private void OnExpChangedEvent(object sender, EventArgs args)
        {
            ExpChangedEvent?.Invoke(this, args);
        }

        private void OnRequireExpChangedEvent(object sender, EventArgs args)
        {
            RequireExpChangedEvent?.Invoke(this, args);
        }

        private void OnLevelUpEvent(object sender, EventArgs args)
        {
            LevelUpEvent?.Invoke(this, args);
        }

        private void OnSkillAddedEvent(object sender, SkillAddedEventArgs args)
        {
            AddSkill(_unitEntity.GetSkill(args.SkillID));
        }

        public void AddSkill(Entity.ISkill skillEntity)
        {
            var skill = _createSkill(skillEntity);
            if (skill == null)
            {
                _logger.Warn($"Failed to add skill : {skillEntity.GetSkillID()}");
                return;
            }
            skill.SetOwner(this);

            _skills.Add(skill.GetSkillID(), skill);
        }

        public int GetRequireExp()
        {
            return _unitEntity.GetRequireExp();
        }

        public int GetExp()
        {
            return _unitEntity.GetExp();
        }

        public int GetLevel()
        {
            return _unitEntity.GetLevel();
        }

        public Core.Entity.ISkill GetSkill(string id)
        {
            return _unitEntity.GetSkill(id);
        }

        public string GetViewID()
        {
            return _unitView.GetObjectID();
        }

        public IEnumerable<Skills.ISkill> GetSkills()
        {
            return _skills.Values;
        }

        public IEnumerable<ITarget> FindTargets(float minRange, float maxRange)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ITarget> FindTargets(float range)
        {
            return _targetFinder.FindInRadius(GetPosition(), range, entry => _targetIdentifier.IsEnemy(entry));
        }

        public IEnumerable<ITarget> FindTargets(Vector2 center, float range)
        {
            return _targetFinder.FindInRadius(center, range, entry => _targetIdentifier.IsEnemy(entry));
        }

        public IEnumerable<ITarget> FindTargets(IEnumerable<string> objectIDs)
        {
            return _targetFinder.FindByViewIDs(objectIDs, entry => _targetIdentifier.IsEnemy(entry));
        }

        public IUnit GetMaster()
        {
            return _master;
        }

        public float GetFollowRange()
        {
            return 3.0f;
        }

        public void SetMaster(IUnit unit)
        {
            _master = unit;
            _master.DestroyEvent += OnMasterDestroyEvent;
            _master.DiedEvent += OnMasterDiedEvent;
        }

        public void UnsetMaster()
        {
            if (_master == null)
                return;
            _master.DestroyEvent -= OnMasterDestroyEvent;
            _master.DiedEvent -= OnMasterDiedEvent;
            _master = null;
        }

        private void OnMasterDestroyEvent(object sender, DestroyEventArgs args)
        {
            UnsetMaster();
        }

        private void OnMasterDiedEvent(object sender, DiedEventArgs args)
        {
            Die();
        }

        public int GetRemainHP()
        {
            return _unitEntity.GetHP();
        }

        public bool IsFullHP()
        {
            return _unitEntity.IsFullHP();
        }

        public IEnumerable<ITarget> FindAllies(float range)
        {
            return _targetFinder.FindInRadius(GetPosition(), range, entry => _targetIdentifier.IsAlly(entry));
        }

        public void Heal(int healAmount)
        {
            _unitEntity.ApplyHeal(healAmount);

            _unitView.PlayHealAnimation(healAmount);
        }

        public IEnumerable<ITarget> FindTargetsInSector(Vector2 center, Vector2 direction, float angle, float range)
        {
            return _targetFinder.FindInSector(center, direction, angle, range, entry => _targetIdentifier.IsEnemy(entry));
        }

        public Skills.ISkill GetDefaultSkill()
        {
            var defaultSkillID = _unitEntity.GetDefaultSkillID();
            if (!string.IsNullOrEmpty(defaultSkillID) &&
                _skills.TryGetValue(defaultSkillID, out var skill))
                return skill;
            return null;
        }

        public void Cast(float castTime)
        {
            _unitView.Stop();
            _unitView.PlayMotion(UnitMotionID.Cast);
        }

        public void Attack(Vector2 direction, float attackTime)
        {
            _unitView.Stop();
            _unitView.PlayMotion(UnitMotionID.Attack, direction, attackTime, true);
        }

        public void SetUnitLogic(string logicID)
        {
            DestroyUnitLogic();

            _unitLogic = _createUnitLogic.Invoke(this, logicID);
            _unitLogic.StartControl(this);
        }

        private void DestroyUnitLogic()
        {
            if (_unitLogic == null)
                return;

            _unitLogic?.Destroy();
            _unitLogic = null;
        }
    }
}
