using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Tránh kích hoạt nhiều lần
        if (hasTriggered) return;

        // Chỉ kích hoạt khi chạm vào Player
        if (collision.CompareTag("Player"))
        {
            hasTriggered = true;

            // Tùy chọn: Bắt Player đứng yên khi chiến thắng
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Gọi hàm hiện Victory Screen
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ActivateVictoryScreen();
            }
            else
            {
                Debug.LogWarning("Không tìm thấy UIManager.Instance để hiện Win Screen!");
            }
        }
    }
}
