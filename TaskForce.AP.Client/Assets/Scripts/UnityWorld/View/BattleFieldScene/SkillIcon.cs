using System.Threading;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class SkillIcon : MonoBehaviour, ISkillIcon
    {
        [SerializeField]
        private Image _iconImage;
        [SerializeField]
        private TMP_Text _levelText;
        [SerializeField]
        private GameObject _iconBack; // 추가된 필드

        public AssetLoader AssetLoader;
        public Core.ILogger Logger;

        private CancellationTokenSource _loadIconToken;
        private string _currentLoadingIconID;

        /// <summary>
        /// 빈 슬롯 상태(배경만 표시)를 보여줍니다.
        /// </summary>
        public void ShowEmpty()
        {
            gameObject.SetActive(true);
            if (_iconBack != null) _iconBack.SetActive(true);
            if (_iconImage != null) _iconImage.gameObject.SetActive(false);
            if (_levelText != null) _levelText.gameObject.SetActive(false);
        }

        /// <summary>
        /// 스킬이 채워진 상태의 비주얼을 복구합니다.
        /// </summary>
        public void ShowFilled()
        {
            gameObject.SetActive(true);
            if (_iconBack != null) _iconBack.SetActive(true);
            if (_iconImage != null) _iconImage.gameObject.SetActive(true);
            if (_levelText != null) _levelText.gameObject.SetActive(true);
        }

        public async void SetIcon(string iconID)
        {
            if (_currentLoadingIconID == iconID)
                return;

            _currentLoadingIconID = iconID;
            
            ShowFilled(); // 아이콘 설정 시 채워진 상태로 전환

            ResetLoadIconToken();
            _loadIconToken = new CancellationTokenSource();
            var token = _loadIconToken.Token;

            Sprite sprite;
            try
            {
                sprite = await AssetLoader.LoadAsset<Sprite>(iconID, token);
            }
            catch (System.OperationCanceledException)
            {
                return;
            }
            catch (System.Exception ex)
            {
                Logger.Warn($"Failed to load icon ({iconID}): {ex.Message}");
                return;
            }

            if (_iconImage && _currentLoadingIconID == iconID)
                _iconImage.sprite = sprite;
        }

        public void SetLevel(int level)
        {
            if (_levelText != null)
            {
                _levelText.text = level.ToString();
            }
        }

        private void ResetLoadIconToken()
        {
            if (_loadIconToken == null)
                return;

            _loadIconToken.Cancel();
            _loadIconToken.Dispose();
            _loadIconToken = null;
        }

        private void OnDestroy()
        {
            ResetLoadIconToken();
        }
    }
}
