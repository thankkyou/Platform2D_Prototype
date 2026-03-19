using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator anim;
    private Collider2D doorCollider;
    private bool isOpen = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;
        gameObject.SetActive(false);

        if (doorCollider != null)
            doorCollider.enabled = false; // Tắt collider để player đi qua
    }

    // public void ResetDoor()
    // {
    //     isOpen = false;
    //     gameObject.SetActive(true);

    //     if (doorCollider != null)
    //         doorCollider.enabled = true;

    //     if (anim != null)
    //         anim.Play("door_closed");
    // }
}