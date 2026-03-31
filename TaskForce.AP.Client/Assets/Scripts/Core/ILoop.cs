namespace TaskForce.AP.Client.Core
{
    public interface ILoop
    {
        void Add(IUpdatable updatable);
        void Remove(IUpdatable updatable);
    }
}
