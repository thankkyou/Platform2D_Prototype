using System.Collections;
using UnityEngine;

public class LadderMovement : MonoBehaviour
{
    private float vertical;
    private float horizontal;
    private float speed = 8f;
    private bool isLadder;
    private bool isClimbing;
    private bool canClimb;        // đang trong vùng ladder
    private bool snappedToLadder;
    private float ladderCenterX;
    [SerializeField] private float jumpForce = 45f;
    private bool jumpPressed;

    [HideInInspector]public PlayerStateList pState;
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
    }

    [SerializeField] private Rigidbody2D rb;

    // Update is called once per frame
    void Update()
    {
        if (pState != null && !pState.alive)
        {
            isClimbing = false;
            canClimb = false;
            snappedToLadder = false;
            rb.gravityScale = 9.5f;
            return;
        }

        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        jumpPressed = Input.GetButtonDown("Jump");

        // Bắt đầu leo khi có input vertical
        if (canClimb && Mathf.Abs(vertical) > 0f)
        {
            isClimbing = true;

            if (!snappedToLadder)
            {
                transform.position = new Vector3(
                    ladderCenterX,
                    transform.position.y,
                    transform.position.z
                );
                snappedToLadder = true;
            }
        }

        // NHẢY RA KHỎI LADDER
        if (isClimbing && jumpPressed)
        {
            ExitLadderAndJump();
        }

        anim.SetBool("Climbing", isClimbing);
    }

    void ExitLadderAndJump()
    {
        isClimbing = false;
        snappedToLadder = false;
        canClimb = false;

        rb.gravityScale = 9.5f;

        // Reset velocity cũ
        rb.linearVelocity = Vector2.zero;

        // Nhảy theo hướng input
        Vector2 jumpDir = new Vector2(horizontal, 1f).normalized;
        rb.AddForce(jumpDir * jumpForce, ForceMode2D.Impulse);
    }


    void FixedUpdate()
    {
        if (pState != null && !pState.alive) return;

        if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.position = new Vector2(ladderCenterX, rb.position.y);
            rb.linearVelocity = new Vector2(0f, vertical * speed);
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            canClimb = true;
            ladderCenterX = collision.bounds.center.x;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            canClimb = false;
            isClimbing = false;
            snappedToLadder = false;
            rb.gravityScale = 9.5f;
        }
    }

}