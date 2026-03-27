using System;

namespace TaskForce.AP.Client.Core.View.Windows
{
    public interface IWindow
    {
        event EventHandler ClosedEvent;
        void Close();
    }
}
