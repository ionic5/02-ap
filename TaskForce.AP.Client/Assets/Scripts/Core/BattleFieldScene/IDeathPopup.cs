using System;

namespace TaskForce.AP.Client.Core.BattleFieldScene
{
    // This would be an interface that a concrete Unity UI component would implement
    // in the Assets/Scrpits/UnityWorld/View/UI/Windows/ folder
    public interface IDeathPopup
    {
        void ShowDeathPopup(Action onRestart, Action onRevive, string unitName);
        void HideDeathPopup();
    }
}