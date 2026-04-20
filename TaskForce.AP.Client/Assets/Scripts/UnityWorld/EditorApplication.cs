using UnityEditor;

namespace TaskForce.AP.Client.UnityWorld
{
#if UNITY_EDITOR
    public class EditorApplication : IApplication
    {
        public void Shutdown()
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
#endif
}
