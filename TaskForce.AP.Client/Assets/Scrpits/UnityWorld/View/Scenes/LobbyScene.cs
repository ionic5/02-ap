using System;
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
        // TODO: JW: 임시값 csv에서 적용
        private int MAX_ENERGY = 5;
        // private int MINUTES_FOR_ENERGY_PLUS = 1;//20; // TODO: JW: 20분으로 수정 
        private int ENERGY_FOR_PLAY = 2;     
        private int MIN_RANK = 1;                
        private int MAX_RANK = 10;
        private int GOLD_FOR_RANK_UP = 100;
        public int ENERGY_ADS_REWARD = 2;
        // "/UserData.json";   // TODO: JW: 파일 경로를 다른 곳에 저장 요
        //
        
        [SerializeField] private Loop _loop;
        [SerializeField] private World world;
        [SerializeField] private View.LobbyScene.WindowStack _windowStack;
        [SerializeField] private View.BattleFieldScene.PausePanel _pausePanel;
        [SerializeField] private CommonWindow _commonWindow;
        
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI rankText;

        [SerializeField] private TextMeshProUGUI energyTimerText;
        // [SerializeField] private Image rankImage;   // TODO: JW: 랭크 이미지 추후 적용
        [SerializeField] private Image[] skillSlots;    
        [SerializeField] private int energyChargeMinutes;
        
        public event EventHandler PauseButtonClickedEvent;
        public event EventHandler PlayButtonClickedEvent;
        public event Action<int, int, int> UpdateUserDataStoreEvent;

        public event EventHandler EnergyGetButtonClickedEvent;
        public event EventHandler CommonWindowOpenedEvent;
        public event EventHandler RankUpWindowOpenedEvent;

        public Loop Loop => _loop;
        public World World => world;
        
        public View.LobbyScene.WindowStack WindowStack => _windowStack;
        public View.BattleFieldScene.PausePanel PausePanel => _pausePanel;
        private UserData _userDataCurrent = new UserData();
        private bool _isEnergyTimerEnable;

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
            
            long timeRemain = energyChargeMinutes * 60 - (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - _userDataCurrent.energyUpdateTime);
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
            userDataTest.rank = MIN_RANK;
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
            _userDataCurrent.gold += 100;
            
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
            if (_userDataCurrent.energy < MAX_ENERGY)
            {
                // 무전기 지급
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                long diffSec = currentTime - _userDataCurrent.energyUpdateTime;
                int energyCount = (int)(diffSec / (energyChargeMinutes * 60f));
                
                if (energyCount > 0)
                {
                    _userDataCurrent.energy = Mathf.Min(_userDataCurrent.energy + energyCount, MAX_ENERGY);
                    _userDataCurrent.energyUpdateTime += energyCount * energyChargeMinutes * 60;
                    UpdateUserDataStore();
                }
            }
            
            bool isEnergyUpdate = _userDataCurrent.energy < MAX_ENERGY;
            energyTimerText.gameObject.SetActive(isEnergyUpdate);
            _isEnergyTimerEnable = isEnergyUpdate;
        }

        public void OnPauseButtonClicked()
        {
            PauseButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnEnergyGetButtonClicked()
        {
            if (_userDataCurrent.energy >= MAX_ENERGY)
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
            if (_userDataCurrent.energy < ENERGY_FOR_PLAY)
            {
                // TODO: JW: text assign 방식 변경
                _commonWindow.SetContentsText("무전기가 부족합니다.");
                
                CommonWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
                
                return; 
            }

            _userDataCurrent.energy -= ENERGY_FOR_PLAY;
            _userDataCurrent.energyUpdateTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            SaveUserDataStore(true);
            
            PlayButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void OnRankUpButtonClicked()
        {
            if (_userDataCurrent.rank >= MAX_RANK)
            {
                // TODO: JW: text assign 방식 변경
                _commonWindow.SetContentsText("최대 계급입니다.");
                
                CommonWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
                
                return; 
            }

            if (_userDataCurrent.gold < GOLD_FOR_RANK_UP)
            {
                // TODO: JW: text assign 방식 변경
                _commonWindow.SetContentsText("골드가 부족합니다.");
                
                CommonWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
                
                return; 
            }
            
            RankUpWindowOpenedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RankUp()
        {
            _userDataCurrent.rank++;
            _userDataCurrent.gold -= GOLD_FOR_RANK_UP;

            updateSkillSlots();
            SaveUserDataStore();
        }

        void updateSkillSlots()
        {
            // TODO: JW: 계급별 오픈된 슬롯 기능 csv에서 적용
            for (int i = 0; i < skillSlots.Length; i++)
            {
                skillSlots[i].gameObject.SetActive(i < _userDataCurrent.rank);
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
            energyText.text = energy + "/" + MAX_ENERGY;
        }

        public void SetRank(int rank)
        {
            _userDataCurrent.rank = rank;
            rankText.text = rank.ToString();
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
        }
    }
}
