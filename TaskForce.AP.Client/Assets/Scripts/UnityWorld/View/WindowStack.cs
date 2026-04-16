using System.Collections.Generic;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View
{
    public class WindowStack : MonoBehaviour
    {
        public event System.EventHandler WindowCountChangedEvent;

        private List<Window> _windows;

        [SerializeField]
        private GameObject _windowBackground;

        void Awake()
        {
            _windows = new List<Window>();
        }

        public T OpenWindow<T>(T window) where T : Window
        {
            window.transform.SetAsLastSibling();
            window.gameObject.SetActive(true);
            _windows.Add(window);

            UpdateWindowBackground();

            window.ClosedEvent += (sender, args) =>
            {
                window.transform.SetSiblingIndex(0);
                _windows.Remove(window);

                UpdateWindowBackground();
            };

            return window;
        }

        public int GetOpenedWindowCount()
        {
            return _windows.Count;
        }

        private void UpdateWindowBackground()
        {
            WindowCountChangedEvent?.Invoke(this, System.EventArgs.Empty);

            if (!IsOpenedWindowExist())
            {
                _windowBackground.SetActive(false);
                return;
            }

            _windowBackground.SetActive(true);

            var lastWindow = _windows[_windows.Count - 1];
            int index = lastWindow.transform.GetSiblingIndex();
            _windowBackground.transform.SetSiblingIndex(index - 1);
        }

        private bool IsOpenedWindowExist()
        {
            return _windows.Count > 0;
        }
    }
}