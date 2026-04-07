using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class PlayerUnitLogic : UnitLogic, IFieldObjectHandler, IFieldItemHandler
    {
        private readonly IJoystick _joystick;
        private readonly IFieldObjectFinder _fieldObjectFinder;
        private readonly List<IFieldObject> _fieldObjects;
        private readonly GameDataStore _gameDataStore;

        private UnitState _state = UnitState.Initial;
        private Skills.ISkill _usingSkill;
        private ITarget _lastTarget;

        public PlayerUnitLogic(ILoop loop, IJoystick joystick, IFieldObjectFinder fieldObjectFinder,
            GameDataStore gameDataStore) : base(loop)
        {
            _joystick = joystick;
            _fieldObjectFinder = fieldObjectFinder;
            _gameDataStore = gameDataStore;
            _fieldObjects = new List<IFieldObject>();
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
            TryHandleFieldObjects();
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

        private void TryHandleFieldObjects()
        {
            int count = _fieldObjectFinder.FindRadius(
                GetControlTarget().GetPosition(),
                GetControlTarget().GetAttribute(AttributeID.DetectRange).AsFloat(),
                _fieldObjects);

            if (count <= 0) return;

            foreach (var fieldObject in _fieldObjects)
                fieldObject.Handle(this);

            _fieldObjects.Clear();
        }

        void IFieldObjectHandler.Handle(ExpOrb orb)
        {
            if (!orb.IsMovingTo(GetControlTarget()))
                orb.MoveTo(GetControlTarget(), 0.5f);
            else if (Vector2.DistanceSquared(GetControlTarget().GetPosition(), orb.GetPosition()) <
                     GetControlTarget().GetPickUpRange() * GetControlTarget().GetPickUpRange())
                AbsorbExpOrb(orb);
        }

        void IFieldObjectHandler.Handle(IFieldItem item)
        {
            item.Handle(this);
        }

        void IFieldItemHandler.Handle(MedicalKit kit)
        {
            if (Vector2.DistanceSquared(GetControlTarget().GetPosition(), kit.GetPosition()) <
                GetControlTarget().GetPickUpRange() * GetControlTarget().GetPickUpRange())
                kit.Use(GetControlTarget());
        }

        void IFieldObjectHandler.Handle(RootBox box)
        {
            TryUseDefaultSkill(box);
        }

        private void AbsorbExpOrb(ExpOrb orb)
        {
            GetControlTarget().AddExp(_gameDataStore.GetSoulExp(orb.GetLevel()));
            orb.Destroy();
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

        protected override void OnDestroy()
        {
            base.OnDestroy();

            ClearLastTarget();
            _usingSkill = null;
        }
    }
}
