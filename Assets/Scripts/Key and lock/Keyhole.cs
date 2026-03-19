using System.Collections;
using UnityEngine;

public class Keyhole : MonoBehaviour
{
    [SerializeField] private Door door;
    private bool playerInRange = false;
    private bool isUnlocked = false;

    private void Update()
    {
        if (playerInRange && !isUnlocked && Input.GetButtonDown("Interact"))
        {
            Debug.Log("Interact pressed | HasAllKeys: " + KeyManager.Instance.HasAllKeys 
            + " | Keys: " + KeyManager.Instance.CollectedKeys 
            + "/" + KeyManager.Instance.RequiredKeys);

            if (KeyManager.Instance.HasAllKeys)
            {
                Unlock();
            }
            else
            {
                KeyManager.Instance.FlashNotEnoughKeys();
            }
        }
    }

    void Unlock()
    {
         if (KeyManager.Instance.HasAllKeys)
        {
            Debug.Log("Unlock called | door: " + door);
            isUnlocked = true;
            door?.OpenDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
         Debug.Log("Trigger entered by: " + collision.gameObject.name + " | tag: " + collision.tag);
        if (collision.CompareTag("Player"))
            playerInRange = false;
            Debug.Log("Player in range!");
    }


}