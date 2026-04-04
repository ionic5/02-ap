using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class RootBox : PoolableObject, Core.View.BattleFieldScene.IRootBox
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

        public string GetObjectID()
        {
            return gameObject.name;
        }
    }
}
