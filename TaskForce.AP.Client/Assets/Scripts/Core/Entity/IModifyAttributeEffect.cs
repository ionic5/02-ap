namespace TaskForce.AP.Client.Core.Entity
{
    public interface IModifyAttributeEffect
    {
        int GetApplyOrder();
        void Apply(AttributeStore store);
        bool CanMerge(IModifyAttributeEffect effect);
        void Merge(IModifyAttributeEffect effect);
        IModifyAttributeEffect Clone();
    }
}
