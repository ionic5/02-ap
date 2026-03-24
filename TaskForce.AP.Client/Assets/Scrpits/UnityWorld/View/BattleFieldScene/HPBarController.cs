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

        private Coroutine _hideHPBarCoroutine;
        private WaitForSeconds _waitSeconds = new WaitForSeconds(1.0f);

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
                Debug.Log("GetComponent Canvas is not null");
                _canvas = canvasObject.GetComponent<Canvas>();
                
            }
            

            // _canvas = GameManager.Instance.Canvas;
            _hpBar = Instantiate(hpBarPrefab, _canvas.transform).GetComponent<HPBar>();
            _hpBarRectTransform = _hpBar.GetComponent<RectTransform>();
            _hpBar.transform.SetAsFirstSibling();
            _offset = new Vector3(0, 1.5f, 0);

            SetActiveHPBar(true);            
        }

        public void SetActiveHPBar(bool active)
        {
            // TODO HP 바 구현 재검토 필요
            if (_hpBar != null)
                _hpBar.gameObject.SetActive(active);
        }

        public void SetHp(float hp)
        {
            _hpBar.SetHPGauge(hp);
            SetActiveHPBar(true);

            if (_hideHPBarCoroutine != null)
            {
                StopCoroutine(_hideHPBarCoroutine);
            }
            else
            {
                _hideHPBarCoroutine = StartCoroutine(HideHPBarAfterDelay());
            }
        }

        IEnumerator HideHPBarAfterDelay()
        {
            yield return _waitSeconds;
            SetActiveHPBar(false);

            _hideHPBarCoroutine = null;
        }

        private void LateUpdate()
        {
            var screenPosition = _camera.WorldToScreenPoint(transform.position + _offset);

            bool isVisible = screenPosition.z > 0
                && screenPosition.x > 0 && screenPosition.x < UnityEngine.Screen.width
                && screenPosition.y > 0 && screenPosition.y < UnityEngine.Screen.height;
            
            if (isVisible)
            {
                _hpBarRectTransform.position = screenPosition;
            }
            else
            {
                SetActiveHPBar(true);
            }
        }
    }
}