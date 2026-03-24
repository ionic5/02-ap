using System;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene
{
    public interface IPausePanel : IDestroyable
    {
        event EventHandler ClickedEvent;
        void Show();
        void Hide();
    }
}
