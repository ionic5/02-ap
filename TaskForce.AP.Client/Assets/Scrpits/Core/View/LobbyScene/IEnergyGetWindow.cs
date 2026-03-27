using System;
using TaskForce.AP.Client.Core.View.BattleFieldScene.Windows;
using TaskForce.AP.Client.Core.View.Windows;

namespace TaskForce.AP.Client.Core.View.LobbyScene.Windows
{
    public interface IEnergyGetWindow : IWindow 
    {
        event EventHandler ConfirmButtonClickedEvent;
        event EventHandler CancelButtonClickedEvent;

        void EnergyGetCompleted();
    }
}