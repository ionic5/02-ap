using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TaskForce.AP.Client.Core.GameData;
using TaskForce.AP.Client.Core.View.Scenes;
using TaskForce.AP.Client.UnityWorld.View.LobbyScene.Windows;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using World = TaskForce.AP.Client.UnityWorld.View.LobbyScene.World;

namespace TaskForce.AP.Client.UnityWorld.View.Scenes
{
    public class LobbyScene : Scene, ILobbyScene  
    {
        [SerializeField] private Loop _loop;
        [SerializeField] private World world;
        [SerializeField] private View.LobbyScene.WindowStack _windowStack;
        [SerializeField] private View.BattleFieldScene.PausePanel _pausePanel;
        [SerializeField] private CommonWindow _commonWindow;
        
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private Image rankImage;

        [SerializeField] private TextMeshProUGUI energyTimerText;
        // [SerializeField] private Image rankImage;   // TODO: JW: 랭크 이미지 추후 적용
        [SerializeField] private Image[] skillSlots;    
        
        public int MaxEnergy { get; set; }
        public int MinutesEnergyCharge { get; set; }
        public int EnergyForPlay { get; set; }
        public int EnergyAdsReward { get; set; }
        
        public List<PlayerRank> PlayerRankData { get; set; }

        public event EventHandler PlayButtonClickedEvent;
        public event Action<int, int, int> UpdateUserDataStoreEvent;

        public event EventHandler EnergyGetButtonClickedEvent;
        public event EventHandler CommonWindowOpenedEvent;
        public event EventHandler RankUpWindowOpenedEvent;
        public event EventHandler PauseButtonClickedEvent;

        public Loop Loop => _loop;
        public World World => world;
        
        public View.LobbyScene.WindowStack WindowStack => _windowStack;
        public View.BattleFieldScene.PausePanel PausePanel => _pausePanel;
        private UserData _userDataCurrent = new UserData();
        private bool _isEnergyTimerEnable;
        
        public AssetLoader AssetLoader;
        private Core.ILogger Logger;
        
        private CancellationTokenSource _loadIconToken;
        private string _currentLoadingRankID;
        
        public void LobbySceneControllerStarted()
        {
            energyTimerText.gameObject.SetActive(false);
            LoadUserDataStore();
        }
        
        void LoadUserDataStore()
        {
            var fileService = new FileService();
            UserData userData = fileService.LoadUserData();

            _userDataCurrent.gold = userData.gold;
            _userDataCurrent.energy = userData.energy;
            _userDataCurrent.rank = userData.rank;
            _userDataCurrent.energyUpdateTime = userData.energyUpdateTime;

            CheckEnergyUpdate();

            UpdateUserDataStore();
            updateSkillSlots();
        }

        private void Update()
        {
            UpdateEnergyRemainTime();
        }

        private void UpdateEnergyRemainTime()
        {
            if (!_isEnergyTimerEnable)
                return;
            
            long timeRemain = MinutesEnergyCharge * 60 - (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _userDataCurrent.energyUpdateTime);
            long min = timeRemain / 60;
            long sec = timeRemain % 60;
            
            energyTimerText.text = $"{min:D2}:{sec:D2}";

            if (timeRemain == 0)
            {
                CheckEnergyUpdate();
            }
        }

        // TODO: JW: 테스트 코드 삭제
        public void OnClickResetButtonForTest()
        {
            
            var fileService = new FileService();
            UserData userDataTest = new UserData();
            userDataTest.gold = 200;
            userDataTest.energy = 3;
            userDataTest.energyUpdateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            userDataTest.rank = PlayerRankData.Min(r => r.Rank);
            fileService.SaveUserData(userDataTest);
            
            UserData userData = fileService.LoadUserData();

            _userDataCurrent.gold = userData.gold;
            _userDataCurrent.energy = userData.energy;
            _userDataCurrent.rank = userData.rank;
            _userDataCurrent.energyUpdateTime = userData.energyUpdateTime;

            CheckEnergyUpdate();
            
            UpdateUserDataStore();
            updateSkillSlots();
        }
        
        // TODO: JW: 테스트 코드 삭제
        public void OnClickGoldPlusButtonForTest()
        {
            _userDataCurrent.gold += 100000;
            
            UpdateUserDataStore();
            updateSkillSlots();
        }

        void SaveUserDataStore(bool isEnergyTimeUpdate = false)
        {
            var fileService = new FileService();
            
            UserData userData = new UserData();
            userData.gold = _userDataCurrent.gold;
            userData.energy = _userDataCurrent.energy;
            userData.rank = _userDataCurrent.rank;

            if (isEnergyTimeUpdate)
            {
                userData.energyUpdateTime = _userDataCurrent.energyUpdateTime;
            }
            
            fileService.SaveUserData(userData);
            
            UpdateUserDataStore();
        }

        public void UpdateUserDataStore()
        {
            UpdateUserDataStoreEvent?.Invoke(_userDataCurrent.gold, _userDataCurrent.energy, _userDataCurrent.rank);
        }

        void CheckEnergyUpdate()
        {
            if (_userDataCurrent.energy < MaxEnergy)
            {
                // 무전기 지급
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long diffSec = currentTime - _userDataCurrent.energyUpdateTime;
                int energyCount = (int)(diffSec / (MinutesEnergyCharge * 60f));
                
                if (energyCount > 0)
                {
                    _userDataCurrent.energy = Mathf.Min(_userDataCurrent.energy + energyCount, MaxEnergy);
                    _userDataCurrent.energyUpdateTime += energyCount * MinutesEnergyCharge * 60;
                    UpdateUserDataStore();
                }
            }
            
            bool isEnergyUpdate = _userDataCurrent.energy < MaxEnergy;
            energyTimerText.gameObject.SetActive(isEnergyUpdate);
            _isEnergyTimerEnable = isEnergyUpdate;
        }

        public void OnPauseButtonClicked()
        {
            PauseButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnEnergyGetButtonClicked()
        {
            if (_userDataCurrent.energy >= MaxEnergy)
            {
                // TODO: JW: text assign 방식 변경
                _commonWindow.SetContentsText("무전기가 Max 입니다.");
                
                CommonWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
                
                return;
            }
            
            // TODO: JW: 광고 보상 기능 추가
            
            EnergyGetButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void OnPlayButtonClicked()
        {
            if (_userDataCurrent.energy < EnergyForPlay)
            {
                // TODO: JW: text assign 방식 변경
                _commonWindow.SetContentsText("무전기가 부족합니다.");
                
                CommonWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
                
                return; 
            }

            _userDataCurrent.energy -= EnergyForPlay;
            _userDataCurrent.energyUpdateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            SaveUserDataStore(true);
            
            PlayButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void OnRankUpButtonClicked()
        {
            if (_userDataCurrent.rank >= PlayerRankData.Max(r => r.Rank))
            {
                // TODO: JW: text assign 방식 변경
                _commonWindow.SetContentsText("최대 계급입니다.");
                
                CommonWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
                
                return; 
            }
            
            var rankNext = _userDataCurrent.rank + 1;
            var playerRankDataNext = PlayerRankData.FirstOrDefault(r => r.Rank == rankNext);
            
            if (_userDataCurrent.gold < playerRankDataNext.UpgradeGold)
            {
                // TODO: JW: text assign 방식 변경
                _commonWindow.SetContentsText("골드가 부족합니다.");
                
                CommonWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
                
                return; 
            }
            
            RankUpWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public async void SetRankIcon(string iconID)
        {
            if (_currentLoadingRankID == iconID)
                return;
            
            _currentLoadingRankID = iconID;

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

            if (rankImage && _currentLoadingRankID == iconID)
                rankImage.sprite = sprite;
        }
        
        private void ResetLoadIconToken()
        {
            if (_loadIconToken == null)
                return;

            _loadIconToken.Cancel();
            _loadIconToken.Dispose();
            _loadIconToken = null;
        }

        public void RankUp()
        {
            _userDataCurrent.rank++;
            
            var playerRankdata = PlayerRankData.FirstOrDefault(r => r.Rank == _userDataCurrent.rank);
            _userDataCurrent.gold -= playerRankdata.UpgradeGold;

            updateSkillSlots();
            SaveUserDataStore();
        }

        void updateSkillSlots()
        {
            // TODO: JW: 계급별 오픈된 슬롯 기능 csv에서 적용
            for (int i = 0; i < skillSlots.Length; i++)
            {
                var playerRankData = PlayerRankData.FirstOrDefault(r => r.Rank == _userDataCurrent.rank);
                skillSlots[i].gameObject.SetActive(i < playerRankData.SlotNum);
            }
        }
        
        public void SetGold(int gold)
        {
            _userDataCurrent.gold = gold;
            goldText.text = gold.ToString();
        }

        public void SetEnergy(int energy)
        {
            _userDataCurrent.energy = energy;
            energyText.text = energy + "/" + MaxEnergy;
        }

        public void SetRank(int rank)
        {
            _userDataCurrent.rank = rank;
            
            SetRankIcon(ConstantID.ICRank + "_" + _userDataCurrent.rank.ToString());
        }

        public void AddEnergyForPlay()
        {
            AddEnergy(EnergyForPlay);
        }
        
        public void AddEnergy(int addEnergy)
        {
            _userDataCurrent.energy += addEnergy;
            _userDataCurrent.energyUpdateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            CheckEnergyUpdate();
            
            SaveUserDataStore(true);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
        
            PauseButtonClickedEvent = null;
            PlayButtonClickedEvent = null;
            UpdateUserDataStoreEvent = null;
            EnergyGetButtonClickedEvent = null;
            CommonWindowOpenedEvent = null;
            RankUpWindowOpenedEvent = null;
            
            ResetLoadIconToken();
        }
    }
}
