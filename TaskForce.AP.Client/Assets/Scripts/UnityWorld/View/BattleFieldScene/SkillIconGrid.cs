using System;
using System.Linq;
using System.Text.RegularExpressions;
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
        private bool _isInitialized;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            int GetNumberFromName(string name)
            {
                if (string.IsNullOrEmpty(name)) return int.MaxValue;
                var match = Regex.Match(name, @"\d+");
                return match.Success ? int.Parse(match.Value) : int.MaxValue;
            }

            if (_slots != null)
            {
                _slots = _slots.OrderBy(s => s != null ? GetNumberFromName(s.name) : int.MaxValue).ToArray();
                foreach (var slot in _slots)
                    if (slot != null) slot.SetActive(false);
            }
            
            if (_icons != null)
            {
                _icons = _icons.OrderBy(i => i != null ? GetNumberFromName(i.gameObject.name) : int.MaxValue).ToArray();
                foreach (var icon in _icons)
                    if (icon != null) icon.gameObject.SetActive(false);
            }

            _isInitialized = true;
        }

        public ISkillIcon AddIcon()
        {
            Initialize();

            if (_icons == null || _nextIndex >= _icons.Length)
                return null;

            var icon = _icons[_nextIndex++];
            if (icon != null)
            {
                icon.ShowFilled();
            }
            return icon;
        }

        public ISkillIcon GetIcon(int index)
        {
            Initialize();
            if (_icons == null || index < 0 || index >= _icons.Length)
                return null;
            return _icons[index];
        }

        public bool IsIconExist(int index)
        {
            return index < _nextIndex;
        }

        public void SetIconSlots(int count)
        {
            Initialize();

            if (_slots == null) return;

            for (var i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null) continue;

                bool shouldActive = i < count;
                _slots[i].SetActive(shouldActive);

                // 슬롯이 활성화될 때, 아직 스킬이 할당되지 않은 슬롯은 빈 배경을 보여줌
                if (shouldActive && i >= _nextIndex)
                {
                    if (i < _icons.Length && _icons[i] != null)
                    {
                        _icons[i].ShowEmpty();
                    }
                }
            }
        }

        private void OnDestroy()
        {
            DestroyedEvent?.Invoke(this, EventArgs.Empty);
        }
    }
}
