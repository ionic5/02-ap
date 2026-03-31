using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class Joystick : IJoystick
    {
        private readonly global::Joystick _joystick;
        private Vector2 _inputVector;

        public Joystick(global::Joystick joystick)
        {
            _inputVector = new Vector2();
            _joystick = joystick;
        }

        public Vector2 GetInputVector()
        {
            _inputVector.Y = _joystick.Vertical;
            _inputVector.X = _joystick.Horizontal;

            return _inputVector;
        }

        public bool IsOnControl()
        {
            return _joystick.Vertical != 0.0f || _joystick.Horizontal != 0.0f;
        }
    }
}
