using System;
using System.Threading;
using TaskForce.AP.Client.Core.View.Scenes;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld.View.Scenes
{
    public class LobbyScene : Scene, ILobbyScene
    {
        [SerializeField] private Loop _loop;
        [SerializeField] private View.LobbyScene.WindowStack _windowStack;
        [SerializeField] private View.BattleFieldScene.PausePanel _pausePanel;

        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private Image rankImage;
        [SerializeField] private TextMeshProUGUI energyTimerText;
        [SerializeField] private Image[] skillSlots;
        [FormerlySerializedAs("_mockSoundPlayer")] [SerializeField] private SoundPlayer _soundPlayer;

        public event EventHandler PlayButtonClickedEvent;
        public event EventHandler EnergyGetButtonClickedEvent;
        public event EventHandler RankUpButtonClickedEvent;
        public event EventHandler PauseButtonClickedEvent;

        public Loop Loop => _loop;
        public View.LobbyScene.WindowStack WindowStack => _windowStack;
        public View.BattleFieldScene.PausePanel PausePanel => _pausePanel;
        public SoundPlayer SoundPlayer => _soundPlayer;

        public AssetLoader AssetLoader;
        public Core.ILogger Logger;

        private CancellationTokenSource _loadIconToken;
        private string _currentLoadingRankID;

        public void OnPauseButtonClicked()
        {
            PauseButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnPlayButtonClicked()
        {
            PlayButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnEnergyGetButtonClicked()
        {
            EnergyGetButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnRankUpButtonClicked()
        {
            RankUpButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetGold(int gold)
        {
            goldText.text = gold.ToString();
        }

        public void SetEnergy(int current, int max)
        {
            energyText.text = current + "/" + max;
        }

        public async void SetRankIcon(string iconID)
        {
            if (_currentLoadingRankID == iconID) return;

            _currentLoadingRankID = iconID;

            ResetLoadIconToken();
            _loadIconToken = new CancellationTokenSource();
            var token = _loadIconToken.Token;

            Sprite sprite;
            try
            {
                sprite = await AssetLoader.LoadAsset<Sprite>(iconID, token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception ex)
            {
                Logger?.Warn($"Failed to load icon ({iconID}): {ex.Message}");
                return;
            }

            if (rankImage && _currentLoadingRankID == iconID)
                rankImage.sprite = sprite;
        }

        public void SetEnergyTimerVisible(bool visible)
        {
            energyTimerText.gameObject.SetActive(visible);
        }

        public void SetEnergyTimer(long remainSeconds)
        {
            long min = remainSeconds / 60;
            long sec = remainSeconds % 60;
            energyTimerText.text = $"{min:D2}:{sec:D2}";
        }

        public void SetSlotCount(int count)
        {
            for (int i = 0; i < skillSlots.Length; i++)
                skillSlots[i].gameObject.SetActive(i < count);
        }

        private void ResetLoadIconToken()
        {
            if (_loadIconToken == null) return;

            _loadIconToken.Cancel();
            _loadIconToken.Dispose();
            _loadIconToken = null;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            PauseButtonClickedEvent = null;
            PlayButtonClickedEvent = null;
            EnergyGetButtonClickedEvent = null;
            RankUpButtonClickedEvent = null;

            ResetLoadIconToken();
        }
    }
}
