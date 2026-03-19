using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string spawnID;

    private void Start()
    {
        // Kiểm tra xem ID của cửa này có khớp với ID mà Player muốn đến không
        if (GameManager.Instance != null && GameManager.Instance.nextSpawnID == spawnID)
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.transform.position = transform.position;
                Debug.Log("Spawned Player at: " + spawnID);
                
                // Giật camera ngay lập tức đến vị trí mới, xoá độ trễ của frame cũ
                CameraFollowAssigner camAssigner = FindFirstObjectByType<CameraFollowAssigner>();
                if (camAssigner != null)
                {
                    camAssigner.AssignPlayer();
                }
            }
        }
    }
}
