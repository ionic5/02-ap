using System;

namespace TaskForce.AP.Client.Core.Entity
{
    public interface ISkill
    {
        event EventHandler<LevelChangedEventArgs> LevelChangedEvent;

        string GetSkillID();
        string GetIconID();
        string GetName();
        string GetDescription();
        int GetLevel();
        void SetLevel(int value);
        void LevelUp();
        void OnAddedToUnit(Unit unit);
        Variant GetAttribute(string attributeID);
    }
}
