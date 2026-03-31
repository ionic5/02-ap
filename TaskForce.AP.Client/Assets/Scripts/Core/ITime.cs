namespace TaskForce.AP.Client.Core
{
    public interface ITime
    {
        long GetCurrentTimeMilliseconds();
        long GetCurrentTime();
        float GetDeltaTime();
        long GetToday();
    }
}
