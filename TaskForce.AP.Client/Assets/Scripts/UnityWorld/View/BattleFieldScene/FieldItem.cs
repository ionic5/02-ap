using System;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class FieldItem : PoolableObject, Core.View.BattleFieldScene.IFieldItem
    {
        public event EventHandler SpawnCompletedEvent;

        [SerializeField]
        private Animator _animator;

        private System.Numerics.Vector2 _position;

        private void OnEnable()
        {
            _animator.Play("spawn");
        }

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

        public void OnSpawnAnimationCompleted()
        {
            SpawnCompletedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
