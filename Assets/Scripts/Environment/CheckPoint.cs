using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    private bool playerInRange;
    public bool interacted;
    private SpriteRenderer sr;

    private PlayerController player;

    AudioManager audioManager;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    void Update()
    {
        if (playerInRange && !interacted)
        {
            ActivateCheckpoint(player);
        }
    }

     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           ActivateCheckpoint(collision.GetComponent<PlayerController>());
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
    void ActivateCheckpoint(PlayerController player)
    {
        interacted = true;
        sr.color = Color.green;
        player.FullRestore();
        GameManager.Instance.SetCheckpoint(this);
        audioManager.PlaySFX(audioManager.checkpoint);
        Debug.Log("Checkpoint activated at: " + transform.position);
    }
}
