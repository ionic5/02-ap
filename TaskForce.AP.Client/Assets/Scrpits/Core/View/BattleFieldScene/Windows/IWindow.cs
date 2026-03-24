using System;

namespace TaskForce.AP.Client.Core.View.BattleFieldScene.Windows
{
    public interface IWindow
    {
        event EventHandler ClosedEvent;
        void Close();
    }
}
