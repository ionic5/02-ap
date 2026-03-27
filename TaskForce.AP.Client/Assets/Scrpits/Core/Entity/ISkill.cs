using System;

namespace TaskForce.AP.Client.Core.Entity
{
    public interface ISkill
    {
        event EventHandler<LevelChangedEventArgs> LevelChangedEvent;

        string GetSkillID();
        string GetIconID();
        string GetName();
        int GetLevel();
        void SetLevel(int value);
        void SetOwner(Unit unit);
        void AddToOwner();
        void LevelUp();
        Attribute GetAttribute(string attributeID);
    }
}
