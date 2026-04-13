using System.Numerics;
using TaskForce.AP.Client.Core.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.BattleFieldScene
{
    public class Joystick : IJoystick
    {
        private readonly global::Joystick _joystick;
        private readonly Camera _camera;
        private System.Numerics.Vector2 _inputVector;

        public Joystick(global::Joystick joystick)
        {
            _joystick = joystick;
            _camera = Camera.main;
            _inputVector = new System.Numerics.Vector2();
        }

        public System.Numerics.Vector2 GetInputVector()
        {
            float h = _joystick.Horizontal;
            float v = _joystick.Vertical;

            if (_camera != null)
            {
                var camForward = UnityEngine.Vector3.ProjectOnPlane(_camera.transform.forward, UnityEngine.Vector3.up).normalized;
                var camRight = UnityEngine.Vector3.ProjectOnPlane(_camera.transform.right, UnityEngine.Vector3.up).normalized;
                var worldDir = camRight * h + camForward * v;
                _inputVector.X = worldDir.x;
                _inputVector.Y = worldDir.z;
            }
            else
            {
                _inputVector.X = h;
                _inputVector.Y = v;
            }

            return _inputVector;
        }

        public bool IsOnControl()
        {
            return _joystick.Vertical != 0.0f || _joystick.Horizontal != 0.0f;
        }
    }
}
