using System;
using System.Collections.Generic;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;
using UnityEngine.AI;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class Unit : PoolableObject, Core.View.BattleFieldScene.IUnit, IDestroyable, IFollowable
    {
        public event EventHandler DieAnimationFinishedEvent;
        public event EventHandler MoveDirectionChangedEvent;

        [SerializeField]
        private NavMeshAgent _agent;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private GameObject _effectAreaCenter;
        [SerializeField]
        private float _pathUpdateThreshold = 0.2f;

        public Func<FloatingTextAnimator> CreateFloatingTextAnimator;
        public Core.Timer Timer;

        private System.Numerics.Vector2 _position;
        private System.Numerics.Vector2 _direction;
        private System.Numerics.Vector2 _moveDirection;

        private Vector3 _destination;
        private bool _isDestinationSetted;

        HPBarController _hpBarController;

        private IReadOnlyDictionary<UnitMotionID, string> _clipNameMap;
        private readonly string[] State = {
            "attack",
            "idle",
            "die",
            "walk",
            "cast"
        };

        private void Awake()
        {
            if (_agent == null)
                _agent = GetComponent<NavMeshAgent>();

            _agent.updateRotation = true;
            _agent.updateUpAxis = true;

            _destination = Vector3.zero;
            _position = new System.Numerics.Vector2();
            _isDestinationSetted = false;

            _clipNameMap = new Dictionary<UnitMotionID, string>
            {
                { UnitMotionID.Attack, State[0] },
                { UnitMotionID.Stand, State[1] },
                { UnitMotionID.Die, State[2] },
                { UnitMotionID.Move, State[3] },
                { UnitMotionID.Cast, State[4] }
            };

            _hpBarController = GetComponent<HPBarController>();
        }

        private void Update()
        {
            if (!_isDestinationSetted) return;

            if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
            {
                if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f)
                {
                    Stop();
                }
            }
            else
            {
                UpdateMoveDirectionFromAgent();
            }
        }

        public void MoveTo(System.Numerics.Vector2 position, float speed)
        {
            Vector3 targetPos = new Vector3(position.X, transform.position.y, position.Y);

            if (!_isDestinationSetted || Vector3.Distance(_destination, targetPos) > _pathUpdateThreshold)
            {
                _destination = targetPos;
                _isDestinationSetted = true;
                _agent.isStopped = false;
                _agent.speed = speed;
                _agent.SetDestination(_destination);
            }
        }

        public void Move(System.Numerics.Vector2 velocity)
        {
            StopMoveTo();
            _agent.isStopped = false;
            _agent.velocity = new Vector3(velocity.X, 0f, velocity.Y);
        }

        public void Stop()
        {
            if (_agent.isOnNavMesh)
            {
                _agent.isStopped = true;
                _agent.velocity = Vector3.zero;
            }
            _isDestinationSetted = false;
            PlayMotion(UnitMotionID.Stand);
        }

        private void StopMoveTo()
        {
            _isDestinationSetted = false;
        }

        private void UpdateMoveDirectionFromAgent()
        {
            if (_agent.velocity.sqrMagnitude < 0.01f) return;

            Vector3 vel = _agent.velocity.normalized;
            System.Numerics.Vector2 currentDir = new System.Numerics.Vector2(vel.x, vel.z);

            if (System.Numerics.Vector2.Distance(_moveDirection, currentDir) > 0.1f)
            {
                _moveDirection = currentDir;
                SetDirection(_moveDirection);
                MoveDirectionChangedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        public System.Numerics.Vector2 GetPosition()
        {
            _position.X = transform.position.x;
            _position.Y = transform.position.z;
            return _position;
        }

        public void SetPosition(System.Numerics.Vector2 position)
        {
            Vector3 newPos = new Vector3(position.X, transform.position.y, position.Y);

            if (_agent.enabled)
                _agent.Warp(newPos);
            else
                transform.position = newPos;
        }

        public System.Numerics.Vector2 GetDirection()
        {
            return _direction;
        }

        public void SetDirection(System.Numerics.Vector2 direction)
        {
            _direction = direction;
        }

        public void PlayMotion(UnitMotionID motionID, System.Numerics.Vector2 direction, float playTime, bool forceRestart)
        {
            SetDirection(direction);
            PlayAnimation(_clipNameMap[motionID], playTime, forceRestart);
        }

        public void PlayMotion(UnitMotionID motionID)
        {
            PlayMotion(motionID, _direction, 1.0f, false);
        }

        private void PlayAnimation(string clipName, float duration = 1.0f, bool forceRestart = false)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(clipName) && !forceRestart) return;
            _animator.speed = 1.0f / duration;

            _animator.Play(clipName, 0, 0f);
        }

        public System.Numerics.Vector2 GetMoveDirection()
        {
            Vector3 velocity = _agent.velocity;
            if (velocity.sqrMagnitude > 0f)
            {
                Vector3 normalized = velocity.normalized;
                _moveDirection.X = normalized.x;
                _moveDirection.Y = normalized.y;
            }
            else
            {
                _moveDirection = System.Numerics.Vector2.Zero;
            }
            return _moveDirection;
        }

        public void PlayDamageAnimation(int damage)
        {
            var animator = CreateFloatingTextAnimator.Invoke();
            var offset = _effectAreaCenter.transform.localPosition;
            animator.Follow(this, new System.Numerics.Vector2(offset.x, offset.y));
            animator.PlayDamageAnimation(damage);
            animator.BringToTop();

            EventHandler hdlr = null;
            hdlr = (sender, args) =>
            {
                animator.AnimationFinishedEvent -= hdlr;
                animator.Destroy();
            };
            animator.AnimationFinishedEvent += hdlr;
        }

        public void PlayHealAnimation(int healAmount)
        {
            var animator = CreateFloatingTextAnimator.Invoke();
            var offset = _effectAreaCenter.transform.localPosition;
            animator.Follow(this, new System.Numerics.Vector2(offset.x, offset.y));
            animator.PlayDamageAnimation(healAmount);
            animator.BringToTop();

            EventHandler hdlr = null;
            hdlr = (sender, args) =>
            {
                animator.AnimationFinishedEvent -= hdlr;
                animator.Destroy();
            };
            animator.AnimationFinishedEvent += hdlr;
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            _hpBarController.SetActiveHPBar(false);
            DieAnimationFinishedEvent = null;
            CreateFloatingTextAnimator = null;
            if (Timer != null)
            {
                Timer.Destroy();
                Timer = null;
            }
        }

        public void OnDieAnimationFinished()
        {
            DieAnimationFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        public string GetObjectID()
        {
            return gameObject.name;
        }
    }
}