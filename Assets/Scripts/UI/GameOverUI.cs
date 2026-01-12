using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
     [SerializeField] private GameObject gameOverPanel;

    private void Awake()
    {
        // ✅ ẨN GAME OVER KHI GAME BẮT ĐẦU
        gameOverPanel.SetActive(false);
    }

    private void Start()
    {
        // Lắng nghe event chết của Player
        PlayerController.Instance.OnDeathAnimationFinished += ShowGameOver;
    }

    private void OnDestroy()
    {
        // Tránh leak event
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnDeathAnimationFinished -= ShowGameOver;
    }

    void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // ⏸️ Dừng game (tuỳ chọn)
    }


   public void RestartButton()
    {
        
    }
}
