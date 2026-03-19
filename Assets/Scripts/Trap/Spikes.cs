using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            PlayerController.Instance.TakeDamage(PlayerController.Instance.maxHealth);
        }
        
    }
}
