using UnityEngine;

public class KeyCollectible : MonoBehaviour
{
    AudioManager audioManager;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            KeyManager.Instance.CollectKey();
            audioManager.PlaySFX(audioManager.playerCollect);
            Destroy(gameObject);
        }
    }
}
