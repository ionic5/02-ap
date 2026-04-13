using System;
using TMPro;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    public class FloatingTextAnimator : PoolableObject
    {
        private static event EventHandler SiblingIndexChangedEvent;
        public event EventHandler AnimationFinishedEvent;

        [SerializeField]
        private TMP_Text _text;
        [SerializeField]
        private Animator _animator;
        [SerializeField]
        private MeshRenderer _renderer;
        [SerializeField]
        private Follower _follower;

        private Camera _mainCamera;

        private class ClipName
        {
            public const string Damage = "damage";
        }

        private void Awake()
        {
            _mainCamera = Camera.main;
            Initialize();
        }

        public override void Revive()
        {
            base.Revive();

            Initialize();
        }

        private void Initialize()
        {
            SiblingIndexChangedEvent += OnSiblingIndexChangedEvent;
        }

        public void Follow(Core.BattleFieldScene.IFollowable target, Vector3 offset)
        {
            _follower.Follow(target, offset);
        }

        public void BringToTop()
        {
            gameObject.transform.SetAsLastSibling();
            SiblingIndexChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void PlayDamageAnimation(int damage)
        {
            var text = damage.ToString();

            PlayAnimation(text, ClipName.Damage);
        }

        private void PlayAnimation(string text, string clipName)
        {
            _text.text = text;

            _animator.Play(clipName, 0, 0f);
        }

        public void OnAnimationFinished()
        {
            AnimationFinishedEvent?.Invoke(this, EventArgs.Empty);
        }

        private void LateUpdate()
        {
            if (_mainCamera != null)
                transform.rotation = _mainCamera.transform.rotation;
        }

        public void OnSiblingIndexChangedEvent(object sender, EventArgs e)
        {
            _renderer.sortingOrder = gameObject.transform.GetSiblingIndex();
        }

        protected override void CleanUp()
        {
            base.CleanUp();

            AnimationFinishedEvent = null;

            SiblingIndexChangedEvent -= OnSiblingIndexChangedEvent;
            _follower.Unfollow();
        }
    }
}
