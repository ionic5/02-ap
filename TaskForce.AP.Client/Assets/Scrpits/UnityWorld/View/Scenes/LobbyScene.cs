using System;
using TaskForce.AP.Client.Core.View.Scenes;
using TaskForce.AP.Client.UnityWorld.View.BattleFieldScene;
using TaskForce.AP.Client.UnityWorld.View.LobbyScene;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TaskForce.AP.Client.UnityWorld.View.Scenes
{
    public class LobbyScene : Scene, ILobbyScene  
    {
        [SerializeField] private Loop _loop;
        [SerializeField] private LobbyWorld _lobbyWorld;
        [SerializeField] private View.BattleFieldScene.WindowStack _windowStack;
        [SerializeField] private PausePanel _pausePanel;
        
        [SerializeField] private TextMeshProUGUI goldText;
        [SerializeField] private TextMeshProUGUI energyText;
        // [SerializeField] private Image rankImage;   // TODO: JW: 랭크 이미지 추후 적용
        // [SerializeField] private Image[] skillSlots;    // TODO: JW: 스킬 슬롯 추후 적용
        public event EventHandler PauseButtonClickedEvent;
        public event EventHandler PlayButtonClickedEvent;
        public event Action<int, int> UpdateUserDataStoreEvent;

        public Loop Loop => _loop;
        public LobbyWorld LobbyWorld => _lobbyWorld;
        public View.BattleFieldScene.WindowStack WindowStack => _windowStack;
        public PausePanel PausePanel => _pausePanel;

        public void LobbySceneControllerStarted()
        {
            UpdateUserDataStore();
        }
        
        void UpdateUserDataStore()
        {
            var fileService = new FileService();
            
            // TODO: JW: 테스트 코드 삭제
            // UserData userDataTest = new UserData();
            // userDataTest.gold = 100;
            // userDataTest.energy = 3;
            // fileService.SaveUserData(userDataTest);
            
            UserData userData = fileService.LoadUserData();
            Debug.Log($"{userData.gold}, {userData.energy}");
            UpdateUserDataStoreEvent?.Invoke(userData.gold, userData.energy);
        }

        public void OnPauseButtonClicked()
        {
            PauseButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnPlayButtonClicked()
        {
            PlayButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public void SetGold(int gold)
        {
            goldText.text = gold.ToString();
        }

        public void SetEnergy(int energy)
        {
            energyText.text = energy.ToString() + "/" + 5; // TODO: JW: max energy 값 csv에서 적용
        }

        void OnClickRadioPlusButton()
        {
            
        }

        void OnClickGradeUpgradeButton()
        {
            
        }
        
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
        
            PauseButtonClickedEvent = null;
            PlayButtonClickedEvent = null;
        }
    }
}
