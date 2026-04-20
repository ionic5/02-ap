using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld
{
#if !UNITY_EDITOR
    public class StandaloneApplication : IApplication
    {
        public void Shutdown()
        {
            Application.Quit();
        }
    }
#endif
}
