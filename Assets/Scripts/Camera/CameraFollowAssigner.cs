using UnityEngine;
using Unity.Cinemachine;

public class CameraFollowAssigner : MonoBehaviour
{
    void Start() {
        AssignPlayer();
    }

    public void AssignPlayer() {
        if (PlayerController.Instance == null) return;
        GameObject player = PlayerController.Instance.gameObject;

        if (player != null) {
            var vcam = GetComponent<CinemachineCamera>();
            if (vcam != null) {
                // Cinemachine 3.x sử dụng cấu trúc Target
                var target = vcam.Target;
                target.TrackingTarget = player.transform;
                vcam.Target = target;
                
                vcam.Follow = player.transform;
                vcam.LookAt = player.transform;
                
                // Đặt trạng thái này về false để ép camera dịch chuyển tức thì tới mục tiêu mà không bị làm mượt
                vcam.PreviousStateIsValid = false;
                Debug.Log("Camera assigned tracking target to: " + player.name);
            } else {
                Debug.LogError("Error: CameraFollowAssigner đang không nằm trên component CinemachineCamera. Hãy kéo thả file script này vào đúng đối tượng 'Cinemachine Camera' trong Hierarchy!");
            }
        }
    }
}
