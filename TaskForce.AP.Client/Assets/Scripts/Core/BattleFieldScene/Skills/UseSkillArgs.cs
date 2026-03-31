using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene.Skills
{
    public struct UseSkillArgs
    {
        public ITarget Target;
        public Action OnCompleted;
    }
}
