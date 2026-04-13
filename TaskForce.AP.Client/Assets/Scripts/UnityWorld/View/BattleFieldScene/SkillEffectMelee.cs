using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class SkillEffectMelee : PoolableObject, ISkillEffect
    {
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private float _offset = 0.7f;
        
        public event EventHandler DestroyEvent;

        private void Update()
        {
            if (!_particleSystem.IsAlive())
            {
                DestroySelf();
            }
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            transform.position = new Vector3(position.X, transform.position.y, position.Y) + transform.forward * _offset;
        }

        public void SetRotation(Vector2 direction)
        {
            Vector3 dir = new Vector3(direction.X, 0f, direction.Y);
            transform.rotation = Quaternion.LookRotation(dir);
        }

        private void DestroySelf()
        {
            DestroyEvent?.Invoke(this, EventArgs.Empty);
            Destroy(gameObject);
        }
    }
}