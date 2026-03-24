using TaskForce.AP.Client.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TaskForce.AP.Client.UnityWorld
{
    public class TitleStarter : MonoBehaviour
    {
        public void OnClickToStartButton()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
