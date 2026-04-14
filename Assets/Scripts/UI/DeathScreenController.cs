using System.Collections;
using UnityEngine;

public class DeathScreenController : MonoBehaviour
{
    [Tooltip("Độ trễ trước khi màn hình chết hiện ra (giây)")]
    [SerializeField] private float showDelay = 0.8f;

    public static DeathScreenController FindCurrent()
    {
        return FindFirstObjectByType<DeathScreenController>(FindObjectsInactive.Include);
    }

    // Coroutine phải chạy trên MonoBehaviour ĐANG ACTIVE vì GameOverOverlay đang inactive
    public void Show(MonoBehaviour runner)
    {
        runner.StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        // WaitForSecondsRealtime: hoạt động kể cả khi timeScale = 0
        yield return new WaitForSecondsRealtime(showDelay);
        gameObject.SetActive(true);
        Debug.Log("[DeathScreen] GameOverOverlay activated.");
    }

    public void OnRespawnClicked()
    {
        Debug.Log("[DeathScreen] Respawn button clicked.");

        // Tự ẩn ngay lập tức
        gameObject.SetActive(false);

        // Gọi hàm respawn trong GameManager
        if (GameManager.Instance != null)
            GameManager.Instance.RespawnPlayer();
        else
            Debug.LogError("[DeathScreen] GameManager.Instance is null! Cannot respawn.");
    }
}
