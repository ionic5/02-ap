using System;

namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface ILobbyScene : IDestroyable
    {
        int MaxEnergy{ get; set; }
        int MinutesEnergyCharge{ get; set; }
        int EnergyForPlay{ get; set; }
        int EnergyAdsReward{ get; set; }
        
        event EventHandler PlayButtonClickedEvent;
        event Action<int, int, int> UpdateUserDataStoreEvent;
        
        event EventHandler EnergyGetButtonClickedEvent;
        event EventHandler CommonWindowOpenedEvent;
        event EventHandler RankUpWindowOpenedEvent;

        void LobbySceneControllerStarted();
        void SetGold(int gold);
        void SetEnergy(int energy);

        void SetRank(int rank);
    }
}