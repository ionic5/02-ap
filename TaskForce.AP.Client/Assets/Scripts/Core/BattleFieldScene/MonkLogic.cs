using System.Linq;
using TaskForce.AP.Client.Core.BattleFieldScene.Skills;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public class MonkLogic : UnitLogic
    {
        private readonly Random _random;
        private UnitState _state;

        public MonkLogic(ILoop loop, Random random) : base(loop)
        {
            _random = random;
            _state = UnitState.Initial;
        }

        private enum UnitState
        {
            Initial,
            Wait,
            Follow,
            Healing
        }

        protected override void OnUpdate()
        {
            switch (_state)
            {
                case UnitState.Initial:
                    Wait();
                    break;

                case UnitState.Wait:
                    HandleWaitState();
                    break;

                case UnitState.Follow:
                    HandleFollowState();
                    break;

                case UnitState.Healing:
                    HandleHealingState();
                    break;
            }
        }


        private void HandleWaitState()
        {
            if (!IsMasterNearby())
                Follow();
            else
                TryHeal();
        }

        private void HandleFollowState()
        {
            if (TryHeal())
                return;

            if (IsMasterNearby())
                Wait();
            else
                Follow();
        }

        private void HandleHealingState()
        {
            if (GetControlTarget().GetDefaultSkill().IsCompleted())
                Wait();
        }

        private void Wait()
        {
            _state = UnitState.Wait;
            GetControlTarget().Wait();
        }

        private void Follow()
        {
            _state = UnitState.Follow;

            var master = GetControlTarget().GetMaster();
            var dest = _random.NextPosition(master.GetPosition(), GetControlTarget().GetFollowRange());
            GetControlTarget().MoveTo(dest);
        }

        private bool TryHeal()
        {
            var skill = GetControlTarget().GetDefaultSkill();
            if (!skill.HasHealEffect() || !skill.IsCooldownFinished())
                return false;

            var target = skill.GetTargetsInRange(GetControlTarget())
                .Where(t => !t.IsFullHP())
                .OrderBy(t => t.GetRemainHP())
                .FirstOrDefault();

            if (target == null) return false;

            skill.Use(new UseSkillArgs { Target = target });

            if (!skill.IsInstantSkill())
                _state = UnitState.Healing;

            return true;
        }

        private bool IsMasterNearby()
        {
            var master = GetControlTarget().GetMaster();
            float range = GetControlTarget().GetFollowRange();
            return System.Numerics.Vector2.DistanceSquared(GetControlTarget().GetPosition(), master.GetPosition()) <= range * range;
        }
    }
}
