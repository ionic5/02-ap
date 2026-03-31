namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    public interface IJoystick
    {
        System.Numerics.Vector2 GetInputVector();
        bool IsOnControl();
    }
}
