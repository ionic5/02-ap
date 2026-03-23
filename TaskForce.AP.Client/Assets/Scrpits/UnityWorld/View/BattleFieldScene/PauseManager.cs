using UnityEngine;

namespace TaskForce.AP.Client.UnityWorld.View.BattleFieldScene
{
    public class PauseManager : MonoBehaviour
    {
        // 어디서든 PauseManager.Instance로 접근 가능하게 만듬
        public static PauseManager Instance { get; private set; }

        public GameObject pausePanel;
        private bool isPaused = false;

        private void Awake()
        {
            // 씬에 이 객체가 하나만 존재하도록 보장
            if (Instance == null)
            {
                Instance = this;
                // 씬이 바뀌어도 파괴되지 않게 하려면 아래 주석 해제
                // DontDestroyOnLoad(gameObject); 
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            UnityEngine.Time.timeScale = isPaused ? 0f : 1f;
            AudioListener.pause = isPaused;
            pausePanel.SetActive(isPaused);
            
            Debug.Log(isPaused ? "게임 일시정지" : "게임 재개");
        }

        // 외부에서 현재 상태를 확인할 때 사용
        public bool IsPaused() => isPaused;
    }
}