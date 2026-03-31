using System;
using System.Linq;
using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;
using TaskForce.AP.Client.Core.Entity;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class NonPlayerUnitLogic : UnitLogic
    {
        private readonly Timer _timer;
        private readonly Core.View.BattleFieldScene.IWorld _world;

        private ITarget _mainTarget;
        private UnitState _state = UnitState.Initial;

        public NonPlayerUnitLogic(ILoop loop, Timer timer, View.BattleFieldScene.IWorld world) : base(loop)
        {
            _timer = timer;
            _world = world;
        }

        private enum UnitState
        {
            Initial,
            Wait,
            UsingSkill,
            MoveToTarget
        }

        protected override void OnUpdate()
        {
            switch (_state)
            {
                case UnitState.Initial:
                    Wait();
                    SetWarpTimer();
                    break;
                case UnitState.Wait:
                    HandleWaitState();
                    break;
                case UnitState.MoveToTarget:
                    HandleMoveToTargetState();
                    break;
            }
        }

        private void HandleMoveToTargetState()
        {
            TryUseDefaultSkill();
        }

        private void HandleWaitState()
        {
            if (!IsValidTarget(_mainTarget))
                if (!TrySetMainTarget())
                    return;

            TryUseDefaultSkill();
        }

        private void TryUseDefaultSkill()
        {
            var skill = GetControlTarget().GetDefaultSkill();
            if (skill == null || !skill.IsCooldownFinished())
                return;

            if (!IsValidTarget(_mainTarget))
            {
                Wait();
                return;
            }

            if (!skill.IsTargetInRange(GetControlTarget(), _mainTarget))
            {
                MoveTo(_mainTarget);
                return;
            }

            skill.Use(new UseSkillArgs { Target = _mainTarget, OnCompleted = OnSkillCompleted });
            _state = UnitState.UsingSkill;
        }

        private void OnSkillCompleted()
        {
            TryUseDefaultSkill();
        }

        private void Wait()
        {
            _state = UnitState.Wait;
            GetControlTarget().Wait();
        }

        private void MoveTo(ITarget target)
        {
            _state = UnitState.MoveToTarget;
            GetControlTarget().MoveTo(target.GetPosition());
        }

        private bool IsValidTarget(ITarget target)
        {
            return target != null && !target.IsDead();
        }

        private bool TrySetMainTarget()
        {
            var enemies = GetControlTarget().FindTargets(GetControlTarget().GetPosition(), GetControlTarget().GetAttribute(AttributeID.DetectRange).AsFloat());
            var target = enemies.OrderBy(e => Vector2.Distance(GetControlTarget().GetPosition(), e.GetPosition())).FirstOrDefault();

            if (target == null)
                return false;

            SetMainTarget(target);
            return true;
        }

        private void SetMainTarget(ITarget target)
        {
            UnsetMainTarget();
            _mainTarget = target;
            _mainTarget.DestroyEvent += OnDestroyMainTargetEvent;
        }

        private void UnsetMainTarget()
        {
            if (_mainTarget == null)
                return;

            _mainTarget.DestroyEvent -= OnDestroyMainTargetEvent;
            _mainTarget = null;
        }

        private void OnDestroyMainTargetEvent(object sender, EventArgs e)
        {
            UnsetMainTarget();
        }

        private void SetWarpTimer()
        {
            if (IsDestroyed())
                return;

            _timer.Start(30.0f, OnWarpTimeFinished);
        }

        private void OnWarpTimeFinished()
        {
            if (IsDestroyed())
                return;

            if (_world.IsOutOfCameraView(GetControlTarget().GetPosition()))
            {
                Wait();
                GetControlTarget().SetPosition(_world.GetWarpPoint());
            }
            SetWarpTimer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UnsetMainTarget();
            _timer.Destroy();
        }
    }
}
