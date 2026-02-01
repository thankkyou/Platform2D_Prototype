using UnityEngine;
using Unity.Cinemachine;

public class CameraFollowAssigner : MonoBehaviour
{
    void Start() {
        AssignPlayer();
    }

    public void AssignPlayer() {
        // Tìm Player theo Tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            var vcam = GetComponent<CinemachineCamera>(); // Hoặc CinemachineVirtualCamera
            vcam.Follow = player.transform;
            vcam.LookAt = player.transform;
            
            // Xóa bộ nhớ vị trí cũ để tránh camera bị "giật" từ (0,0,0)
            vcam.OnTargetObjectWarped(player.transform, player.transform.position - vcam.transform.position); 
        }
    }
}
