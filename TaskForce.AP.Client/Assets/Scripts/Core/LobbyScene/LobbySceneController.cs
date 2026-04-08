using System;
using System.Linq;
using TaskForce.AP.Client.Core.View.BattleFieldScene;
using TaskForce.AP.Client.Core.View.Scenes;

namespace TaskForce.AP.Client.Core.LobbyScene
{
    public class LobbySceneController : IUpdatable
    {
        private readonly ILobbyScene _scene;
        private readonly WindowOpener _windowOpener;
        private readonly GameDataStore _gameDataStore;
        private readonly ILogger _logger;

        private readonly UserDataStore _userDataStore;
        private readonly TextStore _textStore;

        private bool _isDestroyed;

        private event Action _battleFieldSceneLoadEvent;

        public LobbySceneController(ILobbyScene scene, 
            WindowOpener windowOpener, GameDataStore gameDataStore,
            ILogger logger, UserDataStore userDataStore, TextStore textStore, Action battleFieldSceneLoadEvent)
        {
            _scene = scene;
            _windowOpener = windowOpener;
            _gameDataStore = gameDataStore;
            _logger = logger;
            _userDataStore = userDataStore;
            _textStore = textStore;
            _battleFieldSceneLoadEvent = battleFieldSceneLoadEvent;
        }

        public void Start()
        {
            _scene.DestroyEvent += OnDestroySceneEvent;
            _scene.PlayButtonClickedEvent += OnPlayButtonClickedEvent;
            _scene.EnergyGetButtonClickedEvent += OnEnergyGetButtonClickedEvent;
            _scene.RankUpButtonClickedEvent += OnRankUpButtonClickedEvent;
            _scene.PauseButtonClickedEvent += OnPauseButtonClickedEvent;

            InitView();
        }

        public void Update()
        {
            if (_userDataStore.GetEnergy() >= _gameDataStore.GetMaxEnergy()) return;

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long timeRemain = _gameDataStore.GetMinutesEnergyCharge() * 60L
                - (currentTime - _userDataStore.GetEnergyUpdateTime());

            if (timeRemain <= 0)
            {
                TryChargeEnergy();
                _scene.SetEnergy(_userDataStore.GetEnergy(), _gameDataStore.GetMaxEnergy());
                UpdateEnergyTimer();
            }
            else
            {
                _scene.SetEnergyTimer(timeRemain);
            }
        }

        private void InitView()
        {
            TryChargeEnergy();

            _scene.SetGold(_userDataStore.GetGold());
            _scene.SetEnergy(_userDataStore.GetEnergy(), _gameDataStore.GetMaxEnergy());
            UpdateRank();
            UpdateSlotCount();
            UpdateEnergyTimer();
        }

        private void TryChargeEnergy()
        {
            int maxEnergy = _gameDataStore.GetMaxEnergy();
            if (_userDataStore.GetEnergy() >= maxEnergy) return;

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            long diffSec = currentTime - _userDataStore.GetEnergyUpdateTime();
            int minutesCharge = _gameDataStore.GetMinutesEnergyCharge();
            int energyCount = (int)(diffSec / ((long)minutesCharge * 60));

            if (energyCount <= 0) return;

            int newEnergy = Math.Min(_userDataStore.GetEnergy() + energyCount, maxEnergy);
            long newUpdateTime = _userDataStore.GetEnergyUpdateTime() + (long)energyCount * minutesCharge * 60;

            _userDataStore.SetEnergy(newEnergy);
            _userDataStore.SetEnergyUpdateTime(newUpdateTime);
        }

        private void UpdateEnergyTimer()
        {
            _scene.SetEnergyTimerVisible(_userDataStore.GetEnergy() < _gameDataStore.GetMaxEnergy());
        }

        private void UpdateRank()
        {
            var playerRank = _gameDataStore.GetPlayerRanks()
                .FirstOrDefault(r => r.Rank == _userDataStore.GetRank());
            if (playerRank != null)
                _scene.SetRankIcon(playerRank.IconID);
        }

        private void UpdateSlotCount()
        {
            var playerRank = _gameDataStore.GetPlayerRanks()
                .FirstOrDefault(r => r.Rank == _userDataStore.GetRank());
            if (playerRank != null)
                _scene.SetSlotCount(playerRank.SlotNum);
        }

        private void OnPlayButtonClickedEvent(object sender, EventArgs e)
        {
            int energy = _userDataStore.GetEnergy();
            int energyForPlay = _gameDataStore.GetEnergyForPlay();

            if (energy < energyForPlay)
            {
                // 에너지 부족
                _windowOpener.OpenCommonWindow(_textStore.GetText(TextID.EnergyLackDesc));
                return;
            }

            _userDataStore.SetEnergy(energy - energyForPlay);
            _userDataStore.SetEnergyUpdateTime(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _scene.SetEnergy(_userDataStore.GetEnergy(), _gameDataStore.GetMaxEnergy());
            UpdateEnergyTimer();

            _battleFieldSceneLoadEvent?.Invoke();
        }

        private void OnEnergyGetButtonClickedEvent(object sender, EventArgs e)
        {
            if (_userDataStore.GetEnergy() >= _gameDataStore.GetMaxEnergy())
            {
                // 에너지 max
                _windowOpener.OpenCommonWindow(_textStore.GetText(TextID.EnergyMaxDesc));
                return;
            }

            // TODO: JW: 광고 보상 기능 적용 요
            _windowOpener.OpenEnergyGetWindow(OnEnergyGetConfirmed, _textStore.GetText(TextID.AdsWatchDesc));
        }

        private void OnEnergyGetConfirmed()
        {
            AddEnergy(_gameDataStore.GetEnergyAdsReward());
        }

        private void AddEnergy(int amount)
        {
            int maxEnergy = _gameDataStore.GetMaxEnergy();
            int newEnergy = Math.Min(_userDataStore.GetEnergy() + amount, maxEnergy);

            _userDataStore.SetEnergy(newEnergy);
            _userDataStore.SetEnergyUpdateTime(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            _scene.SetEnergy(newEnergy, maxEnergy);
            UpdateEnergyTimer();
        }

        private void OnRankUpButtonClickedEvent(object sender, EventArgs e)
        {
            var ranks = _gameDataStore.GetPlayerRanks();
            int currentRank = _userDataStore.GetRank();

            if (currentRank >= ranks.Max(r => r.Rank))
            {
                // 최대 계급
                _windowOpener.OpenCommonWindow(_textStore.GetText(TextID.RankMaxDesc));
                return;
            }

            var nextRankData = ranks.FirstOrDefault(r => r.Rank == currentRank + 1);
            if (_userDataStore.GetGold() < nextRankData.UpgradeGold)
            {
                // 골드 부족
                _windowOpener.OpenCommonWindow(_textStore.GetText(TextID.GoldLackDesc));
                return;
            }
            
            _windowOpener.OpenRankUpWindow(OnRankUpConfirmed, _textStore.GetText(nextRankData.TextID));
        }

        private void OnRankUpConfirmed()
        {
            int newRank = _userDataStore.GetRank() + 1;
            var rankData = _gameDataStore.GetPlayerRanks().FirstOrDefault(r => r.Rank == newRank);

            _userDataStore.SetRank(newRank);
            _userDataStore.SetGold(_userDataStore.GetGold() - rankData.UpgradeGold);

            _scene.SetGold(_userDataStore.GetGold());
            UpdateRank();
            UpdateSlotCount();
        }

        private void OnPauseButtonClickedEvent(object sender, EventArgs e)
        {
            _windowOpener.OpenSettingWindow();
        }

        private void OnDestroySceneEvent(object sender, EventArgs e)
        {
            Destroy();
        }

        private void Destroy()
        {
            if (_isDestroyed) return;
            _isDestroyed = true;

            _scene.PlayButtonClickedEvent -= OnPlayButtonClickedEvent;
            _scene.EnergyGetButtonClickedEvent -= OnEnergyGetButtonClickedEvent;
            _scene.RankUpButtonClickedEvent -= OnRankUpButtonClickedEvent;
            _scene.PauseButtonClickedEvent -= OnPauseButtonClickedEvent;
        }
    }
}
