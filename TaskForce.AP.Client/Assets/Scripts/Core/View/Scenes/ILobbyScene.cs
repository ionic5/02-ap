using System;

namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface ILobbyScene : IDestroyable
    {
        event EventHandler PlayButtonClickedEvent;
        event EventHandler EnergyGetButtonClickedEvent;
        event EventHandler RankUpButtonClickedEvent;
        event EventHandler PauseButtonClickedEvent;

        void SetGold(int gold);
        void SetEnergy(int current, int max);
        void SetRankIcon(string iconID);
        void SetEnergyTimerVisible(bool visible);
        void SetEnergyTimer(long remainSeconds);
        void SetSlotCount(int count);
    }
}
