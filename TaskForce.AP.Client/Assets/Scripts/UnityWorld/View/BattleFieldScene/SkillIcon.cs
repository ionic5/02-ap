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

        public AssetLoader AssetLoader;
        public Core.ILogger Logger;

        private CancellationTokenSource _loadIconToken;
        private string _currentLoadingIconID;

        public async void SetIcon(string iconID)
        {
            if (_currentLoadingIconID == iconID)
                return;

            _currentLoadingIconID = iconID;

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
            _levelText.text = level.ToString();
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
