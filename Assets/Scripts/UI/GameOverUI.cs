// using UnityEngine;
// using UnityEngine.SceneManagement;

// public class GameOverUI : MonoBehaviour
// {
//      [SerializeField] private GameObject gameOverPanel;

//     //ẨN GAME OVER KHI GAME BẮT ĐẦU
//     private void Awake()
//     {
        
//         gameOverPanel.SetActive(false);
//     }

//     //Lắng nghe event chết của Player
//     private void Start()
//     {
//         PlayerController.Instance.OnDeathAnimationFinished += ShowGameOver;
//     }

//     // Tránh leak event
//     private void OnDestroy()
//     {
//         if (PlayerController.Instance != null)
//             PlayerController.Instance.OnDeathAnimationFinished -= ShowGameOver;
//     }

//     //Dừng game
//     void ShowGameOver()
//     {
//         gameOverPanel.SetActive(true);
//         Time.timeScale = 0f; //Dừng game
//     }


//    public void RestartButton()
//     {
        
//     }
// }
