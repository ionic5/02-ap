using System;
using System.Threading;
using TaskForce.AP.Client.Core.View.BattleFieldScene.LevelUpWindow;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene.LevelUpWindow
{
    public class SkillPanel : MonoBehaviour, ISkillPanel
    {
        [SerializeField]
        private TMP_Text _descriptionText;
        [SerializeField]
        private TMP_Text _levelText;
        [SerializeField]
        private TMP_Text _nameText;
        [SerializeField]
        private Image _iconImage;
        [SerializeField]
        private Toggle _toggle;

        public AssetLoader AssetLoader;
        public Core.ILogger Logger;

        private CancellationTokenSource _loadIconToken;
        private string _currentLoadingIconID;

        public bool IsSelected()
        {
            return _toggle.isOn;
        }

        public void SetDescription(string value)
        {
            _descriptionText.text = value;
        }

        public async void SetIcon(string iconID)
        {
            if (_currentLoadingIconID == iconID)
                return;

            _currentLoadingIconID = iconID;

            var iconSprite = await LoadIcon(iconID);
            if (iconSprite == null)
                return;

            if (_iconImage && _currentLoadingIconID == iconID)
                _iconImage.sprite = iconSprite;
        }

        private async System.Threading.Tasks.Task<Sprite> LoadIcon(string iconID)
        {
            ResetLoadIconToken();

            _loadIconToken = new CancellationTokenSource();
            CancellationToken token = _loadIconToken.Token;

            try
            {
                return await AssetLoader.LoadAsset<Sprite>(iconID, token);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (System.Exception ex)
            {
                Logger.Warn($"Failed to load icon ({iconID}): {ex.Message}");
                return null;
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

        public void SetLevel(string value)
        {
            _levelText.text = value;
        }

        public void SetName(string value)
        {
            _nameText.text = value;
        }

        public void SetSelected()
        {
            _toggle.isOn = true;
        }
    }
}