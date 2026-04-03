﻿using System;
using System.Collections; // [REVIVE_EFFECT_TEST]
using System.Collections.Generic;
using TaskForce.AP.Client.Core;
using TaskForce.AP.Client.Core.BattleFieldScene;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class Unit : PoolableObject, Core.View.BattleFieldScene.IUnit, IDestroyable, IFollowable
    {
        public event EventHandler DieAnimationFinishedEvent;
        public event EventHandler MoveDirectionChangedEvent;
        public event EventHandler DeathAnimationCompletedEvent; // New event

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
        Core.ILogger _logger;

        private IReadOnlyDictionary<UnitMotionID, string> _clipNameMap;
        private readonly string[] State = {
            "attack",
            "Idle",
            "Dying",
            "Walk",
            "cast"
        };

        private void Awake()
        {
            if (_agent == null)
                _agent = GetComponent<NavMeshAgent>();

            _agent.updateRotation = false;
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

            if (velocity.LengthSquared() > 0f)
            {
                var dir = System.Numerics.Vector2.Normalize(velocity);
                SetDirection(dir);
                transform.rotation = Quaternion.LookRotation(new Vector3(velocity.X, 0f, velocity.Y));
            }
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
                transform.rotation = Quaternion.LookRotation(new Vector3(vel.x, 0f, vel.z));
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

            // TODO HP 바 구현 재검토 필요
            if (_hpBarController != null)
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
            // Instead of immediate deactivation, signal that death animation is complete
            DeathAnimationCompletedEvent?.Invoke(this, EventArgs.Empty);
            if (_hpBarController != null)
                _hpBarController.SetActiveHPBar(false); // Ensure HP bar is hidden

            
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        public string GetObjectID()
        {
            return gameObject.name;
        }

        public void Die()
        {
            throw new NotImplementedException();
        }

        public void SetHPBarVisible(bool visible)
        {
            if (_hpBarController != null)
                _hpBarController.SetActiveHPBar(visible);
        }

        public void SetHpRatio(float ratio)
        {
            if (_hpBarController != null)
                _hpBarController.SetHp(ratio);
        }

        // [REVIVE_EFFECT_TEST] START
        public void PlayReviveEffect(Action onCompleted)
        {
            StartCoroutine(ReviveEffectCoroutine(onCompleted));
        }

        private IEnumerator ReviveEffectCoroutine(Action onCompleted)
        {
            float duration = 2.0f; // 깜빡이는 총 시간
            float blinkInterval = 0.1f; // 깜빡이는 주기
            float elapsed = 0f;

            Vector3 originalScale = transform.localScale;
            transform.localScale = originalScale * 1.3f; // 1.3배 확대

            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            while (elapsed < duration)
            {
                // 사용자가 화면을 클릭하여 일시정지가 해제되면 깜빡임 연출 즉시 중단
                if (UnityEngine.Time.timeScale > 0f)
                    break;

                foreach (var r in renderers) { r.enabled = !r.enabled; }
                yield return new WaitForSecondsRealtime(blinkInterval);
                elapsed += blinkInterval;
            }

            // 크기 및 렌더러 복구
            foreach (var r in renderers) { r.enabled = true; }
            transform.localScale = originalScale;

            onCompleted?.Invoke();
        }
        // [REVIVE_EFFECT_TEST] END
    }
}