using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class FieldItem : PoolableObject, Core.View.BattleFieldScene.IFieldItem
    {
        private System.Numerics.Vector2 _position;

        public System.Numerics.Vector2 GetPosition()
        {
            _position.X = transform.position.x;
            _position.Y = transform.position.z;
            return _position;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, transform.position.y, position.Y);
        }
    }
}
