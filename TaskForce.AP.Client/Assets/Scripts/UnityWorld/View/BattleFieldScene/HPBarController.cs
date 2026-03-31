using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class HPBarController : MonoBehaviour
    {
        [SerializeField] private GameObject hpBarPrefab;
        public HPBar _hpBar;
        private Canvas _canvas;
        private Camera _camera;
        private RectTransform _hpBarRectTransform;
        private Vector3 _offset;

        private bool _isHpBarEnabled = true;
        private float _currentHpRatio = 1.0f;

        private void Start()
        {
            _camera = Camera.main;

            // Searching for Canvas Tag
            var canvasObject = GameObject.FindGameObjectWithTag("Canvas");
            if (!canvasObject)
            {
                canvasObject = new GameObject("Canvas");
                canvasObject.AddComponent<Canvas>();
                canvasObject.AddComponent<CanvasScaler>();
                canvasObject.AddComponent<GraphicRaycaster>();

                _canvas = canvasObject.GetComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.tag = "Canvas";
            }
            else
            {
                _canvas = canvasObject.GetComponent<Canvas>();
            }
            
            _hpBar = Instantiate(hpBarPrefab, _canvas.transform).GetComponent<HPBar>();
            _hpBarRectTransform = _hpBar.GetComponent<RectTransform>();
            _hpBar.transform.SetAsFirstSibling();
            _offset = new Vector3(0, 1.5f, 0);

            _hpBar.SetHPGauge(_currentHpRatio);
            _hpBar.gameObject.SetActive(_isHpBarEnabled);
        }

        public void SetActiveHPBar(bool active)
        {
            _isHpBarEnabled = active;
            if (_hpBar != null)
                _hpBar.gameObject.SetActive(active);
        }

        public void SetHp(float hp)
        {
            _currentHpRatio = hp;
            if (_hpBar != null)
            {
                _hpBar.SetHPGauge(hp);
            }
        }

        private void OnDestroy()
        {
            if (_hpBar != null)
                Destroy(_hpBar.gameObject);
        }

        private void LateUpdate()
        {
            if (_camera == null || _hpBar == null) return;

            var screenPosition = _camera.WorldToScreenPoint(transform.position + _offset);

            bool isVisible = screenPosition.z > 0
                && screenPosition.x > -50 && screenPosition.x < _camera.pixelWidth + 50
                && screenPosition.y > -50 && screenPosition.y < _camera.pixelHeight + 50;
            
            if (isVisible)
            {
                _hpBarRectTransform.position = screenPosition;
                if (_isHpBarEnabled && !_hpBar.gameObject.activeSelf)
                    _hpBar.gameObject.SetActive(true);
            }
            else
            {
                if (_hpBar.gameObject.activeSelf)
                    _hpBar.gameObject.SetActive(false);
            }
        }
    }
}