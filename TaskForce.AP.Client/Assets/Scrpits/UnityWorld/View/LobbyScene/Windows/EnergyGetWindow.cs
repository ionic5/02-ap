using System;
using TaskForce.AP.Client.Core.View.LobbyScene.Windows;
using UnityEngine; 

namespace TaskForce.AP.Client.UnityWorld.View.LobbyScene.Windows
{
    public class EnergyGetWindow : Window, IEnergyGetWindow
    {
        [SerializeField] private Scenes.LobbyScene _lobbyScene;
        
        public event EventHandler ConfirmButtonClickedEvent;
        public event EventHandler CancelButtonClickedEvent;
        
        public void EnergyGetCompleted()
        {
            // 광고 보상 에너지 n개 지급하고 닫음 
            _lobbyScene.AddEnergy(_lobbyScene.ENERGY_ADS_REWARD);   // TODO: JW: 광고 보상 에너지 수치 csv 에서 적용 요
            
            Close();
        }

        public void OnClickConfirmButton()
        {
            ConfirmButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnClickCancelButton()
        {
            CancelButtonClickedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        public override void Clear()
        {
            base.Clear();
        
            ConfirmButtonClickedEvent = null;
            CancelButtonClickedEvent = null;
        }
    }
}