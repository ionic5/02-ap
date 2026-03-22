namespace TaskForce.AP.Client.Core.Entity
{
    public interface ISkill
    {
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
