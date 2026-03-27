using System;

namespace TaskForce.AP.Client.Core.Entity
{
    public class LevelChangedEventArgs : EventArgs
    {
        public string SkillID { get; }
        public int Level { get; }

        public LevelChangedEventArgs(string skillID, int level)
        {
            SkillID = skillID;
            Level = level;
        }
    }
}
