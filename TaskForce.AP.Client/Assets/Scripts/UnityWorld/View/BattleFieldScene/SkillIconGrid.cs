using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class SkillIconGrid : MonoBehaviour, ISkillIconGrid
    {
        public event EventHandler DestroyedEvent;

        [SerializeField]
        private GameObject[] _slots;
        [SerializeField]
        private SkillIcon[] _icons;

        public SkillIcon[] Icons => _icons;

        private int _nextIndex;

        private void Awake()
        {
            foreach (var slot in _slots)
                slot.SetActive(false);
        }

        public ISkillIcon AddIcon()
        {
            if (_nextIndex >= _icons.Length)
                return null;

            var icon = _icons[_nextIndex++];
            icon.gameObject.SetActive(true);
            return icon;
        }

        public ISkillIcon GetIcon(int index)
        {
            return _icons[index];
        }

        public bool IsIconExist(int index)
        {
            return index < _nextIndex;
        }

        public void SetIconSlots(int count)
        {
            for (var i = 0; i < _slots.Length; i++)
                _slots[i].SetActive(i < count);
        }

        private void OnDestroy()
        {
            DestroyedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
