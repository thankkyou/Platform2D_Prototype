using UnityEngine;

public class CheckPoint : MonoBehaviour
{
//    [SerializeField] private string checkpointID;

//     private void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             PlayerController player = other.GetComponent<PlayerController>();
//             if (player != null)
//             {
//                 ActivateCheckpoint(player);
//             }
//         }
//     }

//      void ActivateCheckpoint(PlayerController player)
//     {
//        // Lưu vị trí hồi sinh (mỗi lần va chạm đều cập nhật checkpoint gần nhất)
//         GameManager.Instance.nextSpawnID = checkpointID;

//         // Heal đầy máu, đầy bình, đầy stamina NGAY LẬP TỨC
//         player.Health = player.maxHealth;
//         player.currentHealPotions = player.maxHealPotions;

//         // Reset stamina về full (giả sử StaminaController có method ResetStamina() hoặc FillStamina())
//         var stamina = player.GetComponent<StaminaController>();
//         stamina?.ResetStamina();  // Thay bằng tên method đúng nếu khác (ví dụ: stamina?.FillToMax())

//         Debug.Log($"Checkpoint {checkpointID} activated: Player fully healed, potions refilled, stamina reset!");

//         // (Optional) Hiệu ứng: Âm thanh, particle, animation...
//         // Ví dụ: GetComponent<Animator>()?.SetTrigger("Active");
//         // Hoặc Instantiate(effectPrefab, transform.position, Quaternion.identity);
//     }
    private bool playerInRange;
    public bool interacted;

    private PlayerController player;

    void Start()
    {
        
    }

    void Update()
    {
        if (playerInRange && !interacted && Input.GetKeyDown(KeyCode.E))
        {
            ActivateCheckpoint();
        }
    }

     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
            player = collision.GetComponent<PlayerController>();

            Debug.Log("Player entered checkpoint");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;

            Debug.Log("Player left checkpoint");
        }
    }
    void ActivateCheckpoint()
    {
        interacted = true;

        player.FullRestore(); // nếu bạn có hàm này

        GameManager.Instance.SetCheckpoint(this);

        Debug.Log("Checkpoint activated");
    }
}
