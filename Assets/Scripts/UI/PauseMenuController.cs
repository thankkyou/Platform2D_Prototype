using UnityEngine;

/// <summary>
/// Gắn vào PauseMenu. Các nút trong Pause Menu gọi các method của script này
/// thay vì gọi trực tiếp GameManager — tránh lỗi khi GameManager bị DDOL.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    public void Resume()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.UnPausedGame();
        else
            Debug.LogWarning("[PauseMenuController] GameManager.Instance is null!");
    }
}
