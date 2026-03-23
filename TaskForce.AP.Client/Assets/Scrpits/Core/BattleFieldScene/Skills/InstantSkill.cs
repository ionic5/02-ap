namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public abstract class InstantSkill : ActiveSkill
    {
        protected InstantSkill(Entity.ISkill skillEntity) : base(skillEntity)
        {
        }

        public override bool IsInstantSkill()
        {
            return true;
        }

        public override bool IsCompleted()
        {
            return true;
        }

        public override void Cancel()
        {
        }
    }
}
