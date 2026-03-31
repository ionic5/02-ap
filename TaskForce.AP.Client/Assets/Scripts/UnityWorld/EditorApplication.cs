namespace TaskForce.AP.Client.UnityWorld
{
    public class EditorApplication : IApplication
    {
        public void Shutdown()
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
    }
}
