using System;

namespace TaskForce.AP.Client.Core.View.Scenes
{
    public interface ITitleScene : IDestroyable
    {
        event EventHandler StartButtonClickedEvent;
    }
}
