using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool playerInRange;
    public bool interacted;

    private PlayerController player;

    void Start()
    {
        
    }

    void Update()
    {
        if (playerInRange && !interacted && Input.GetButtonDown("Interact"))
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
