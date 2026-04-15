using System.Collections.Generic;
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
        private readonly UserDataStore _userDataStore;

        private UnitState _state = UnitState.Initial;

        public PlayerUnitLogic(ILoop loop, IJoystick joystick, IFieldObjectFinder fieldObjectFinder,
            UserDataStore userDataStore) : base(loop)
        {
            _joystick = joystick;
            _fieldObjectFinder = fieldObjectFinder;
            _userDataStore = userDataStore;
            _fieldObjects = new List<IFieldObject>();
        }

        private enum UnitState
        {
            Initial,
            Wait,
            Move
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
            {
                if (skill.IsInstantSkill() && skill.IsCooldownFinished())
                    skill.Use(new UseSkillArgs());
            }
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
            if (_state == UnitState.Initial)
                Wait();
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

        void IFieldItemHandler.Handle(GoldBundle bundle)
        {
            if (Vector2.DistanceSquared(GetControlTarget().GetPosition(), bundle.GetPosition()) <
                GetControlTarget().GetPickUpRange() * GetControlTarget().GetPickUpRange())
                bundle.Use(_userDataStore);
        }

        void IFieldItemHandler.Handle(Nuke nuke)
        {
            if (Vector2.DistanceSquared(GetControlTarget().GetPosition(), nuke.GetPosition()) <
                GetControlTarget().GetPickUpRange() * GetControlTarget().GetPickUpRange())
                nuke.Use(GetControlTarget());
        }

        void IFieldItemHandler.Handle(Magnet magnet)
        {
            if (Vector2.DistanceSquared(GetControlTarget().GetPosition(), magnet.GetPosition()) <
                GetControlTarget().GetPickUpRange() * GetControlTarget().GetPickUpRange())
                magnet.Use(GetControlTarget());
        }

        void IFieldObjectHandler.Handle(RootBox box)
        {
        }

        private void AbsorbExpOrb(ExpOrb orb)
        {
            orb.Use(GetControlTarget());
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
            _state = state;
        }

    }
}
