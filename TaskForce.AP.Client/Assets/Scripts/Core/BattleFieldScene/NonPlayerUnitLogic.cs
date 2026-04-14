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
        private readonly float _warpDistanceSq;
        private readonly float _warpCheckInterval;

        private ITarget _mainTarget;
        private UnitState _state = UnitState.Initial;

        public NonPlayerUnitLogic(ILoop loop, Timer timer, GameDataStore gameDataStore, View.BattleFieldScene.IWorld world) : base(loop)
        {
            _timer = timer;
            _world = world;
            var warpDistance = gameDataStore.GetConstant(GameData.ConstantID.WarpDistance).AsFloat();
            _warpDistanceSq = warpDistance * warpDistance;
            _warpCheckInterval = gameDataStore.GetConstant(GameData.ConstantID.WarpCheckInterval).AsFloat();
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
                    SetWarpCheckTimer();
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

        private void SetWarpCheckTimer()
        {
            if (IsDestroyed())
                return;

            _timer.Start(_warpCheckInterval, OnWarpCheckTimerFinished);
        }

        private void OnWarpCheckTimerFinished()
        {
            if (IsDestroyed())
                return;

            if (IsValidTarget(_mainTarget))
            {
                var distanceSq = Vector2.DistanceSquared(GetControlTarget().GetPosition(), _mainTarget.GetPosition());
                if (distanceSq >= _warpDistanceSq)
                {
                    Wait();
                    GetControlTarget().SetPosition(_world.GetNextSpawnPoint());
                }
            }

            SetWarpCheckTimer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            UnsetMainTarget();
            _timer.Destroy();
        }
    }
}
