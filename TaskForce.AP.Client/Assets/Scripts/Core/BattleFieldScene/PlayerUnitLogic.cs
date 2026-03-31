using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class PlayerUnitLogic : UnitLogic
    {
        private readonly IJoystick _joystick;
        private readonly ISoulFinder _soulFinder;
        private readonly List<Soul> _souls;
        private readonly GameDataStore _gameDataStore;

        private UnitState _state = UnitState.Initial;
        private Skills.ISkill _usingSkill;
        private ITarget _lastTarget;

        public PlayerUnitLogic(ILoop loop, IJoystick joystick, ISoulFinder soulFinder,
            GameDataStore gameDataStore) : base(loop)
        {
            _joystick = joystick;
            _soulFinder = soulFinder;
            _gameDataStore = gameDataStore;
            _souls = new List<Soul>();
        }

        private enum UnitState
        {
            Initial,
            Wait,
            Move,
            UsingSkill
        }

        protected override void OnUpdate()
        {
            UpdateState();
            HandleInput();
            UseInstantSkills();
            TryAbsorbSouls();
        }

        private void UseInstantSkills()
        {
            foreach (var skill in GetControlTarget().GetSkills())
                if (skill.IsInstantSkill() && skill.IsCooldownFinished())
                    skill.Use(new UseSkillArgs());
        }

        private void HandleInput()
        {
            if (_joystick.IsOnControl())
                Move(Vector2.Normalize(_joystick.GetInputVector()));
            else if (_state == UnitState.Move)
                Wait();
        }

        private void UpdateState()
        {
            switch (_state)
            {
                case UnitState.Initial:
                    Wait();
                    break;

                case UnitState.Wait:
                    HandleWaitState();
                    break;
            }
        }

        private void TryUseDefaultSkill(ITarget target)
        {
            if (GetControlTarget().GetDefaultSkill() == null || !GetControlTarget().GetDefaultSkill().IsCooldownFinished())
                return;

            SetState(UnitState.UsingSkill);

            _usingSkill = GetControlTarget().GetDefaultSkill();
            SetLastTarget(target);

            _usingSkill.Use(new UseSkillArgs
            {
                Target = _lastTarget,
                OnCompleted = OnSkillCompleted
            });
        }

        private void OnSkillCompleted()
        {
            if (IsLastTargetExist() && _lastTarget.IsAlive())
                TryUseDefaultSkill(_lastTarget);
            else
                Wait();
        }

        private void HandleWaitState()
        {
            var skill = GetControlTarget().GetDefaultSkill();
            var targets = skill?.GetTargetsInRange(GetControlTarget());

            if (targets != null && targets.Any())
            {
                var target = targets.OrderBy(t => Vector2.DistanceSquared(GetControlTarget().GetPosition(), t.GetPosition())).First();
                TryUseDefaultSkill(target);
            }
        }

        private void TryAbsorbSouls()
        {
            int count = _soulFinder.FindRadius(GetControlTarget().GetPosition(), GetControlTarget().GetAttribute(AttributeID.DetectRange).AsFloat(), _souls);
            if (count <= 0) return;

            foreach (var soul in _souls)
            {
                if (!soul.IsMovingTo(GetControlTarget()))
                    soul.MoveTo(GetControlTarget(), 0.5f);
                else if (Vector2.DistanceSquared(GetControlTarget().GetPosition(), soul.GetPosition()) < GetControlTarget().GetPickUpRange() * GetControlTarget().GetPickUpRange())
                    Absorb(soul);
            }
            _souls.Clear();
        }

        private void Move(Vector2 direction)
        {
            SetState(UnitState.Move);
            GetControlTarget().Move(direction);
        }

        private void Wait()
        {
            SetState(UnitState.Wait);
            if(GetControlTarget() != null)
                GetControlTarget().Wait();
        }

        private void SetState(UnitState state)
        {
            var oldState = _state;
            _state = state;

            if (oldState == UnitState.UsingSkill &&
                _usingSkill != null && !_usingSkill.IsCompleted())
            {
                _usingSkill?.Cancel();
                _usingSkill = null;

                ClearLastTarget();
            }
        }

        private void SetLastTarget(ITarget target)
        {
            ClearLastTarget();

            _lastTarget = target;
            _lastTarget.DestroyEvent += OnDestroyLastTargetEvent;
        }

        private void ClearLastTarget()
        {
            if (_lastTarget == null)
                return;

            _lastTarget.DestroyEvent -= OnDestroyLastTargetEvent;
            _lastTarget = null;
        }

        private void OnDestroyLastTargetEvent(object sender, EventArgs e)
        {
            ClearLastTarget();
        }

        private bool IsLastTargetExist()
        {
            return _lastTarget != null;
        }

        private void Absorb(Soul soul)
        {
            GetControlTarget().AddExp(_gameDataStore.GetSoulExp(soul.GetLevel()));
            soul.Destroy();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearLastTarget();
            _usingSkill = null;
        }
    }
}
