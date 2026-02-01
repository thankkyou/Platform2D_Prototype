using UnityEngine;

public class QuyNhapTrang : Enemy
{
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask whatIsGround;

    float timer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        base.Start();
        ChangeState(EnemyStates.QuyNhapTrang_Idle);
        rb.gravityScale = 12f;
    }
    protected override void Awake()
    {
        base.Awake(); 
    }

    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.QuyNhapTrang_Idle);
        }
        
    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Destroy(gameObject, 0.05f);
        }

        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.QuyNhapTrang_Idle:


                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround) 
                || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.QuyNhapTrang_Suprised);
                }

                if (transform.localScale.x > 0)
                {
                    rb.linearVelocity = new Vector2(speed, transform.localScale.y);
                }
                else
                {
                    rb.linearVelocity = new Vector2(-speed, transform.localScale.y);
                }
                break;

            case EnemyStates.QuyNhapTrang_Suprised:
                rb.linearVelocity = new Vector2(0, jumpForce);
                
                ChangeState(EnemyStates.QUyNhapTrang_Charge);
                break;

            case EnemyStates.QUyNhapTrang_Charge:
                timer += Time.deltaTime;

                if (timer < chargeDuration)
                {
                    if(Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        if (transform.localScale.x > 0)
                        {
                            rb.linearVelocity = new Vector2(speed * chargeSpeedMultiplier, rb.linearVelocity.y);
                        } 
                        else
                        {
                            rb.linearVelocity = new Vector2(-speed * chargeSpeedMultiplier, rb.linearVelocity.y);
                        }
                    }
                    else
                    {
                        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.QuyNhapTrang_Idle);
                }
                break;
        }
    }

    protected override void ChangeCurrentAnimation()
    {
        if(GetCurrentEnemyState == EnemyStates.QuyNhapTrang_Idle)
        {
            anim.speed = 1;
        }

        if(GetCurrentEnemyState == EnemyStates.QUyNhapTrang_Charge)
        {
            anim.speed = chargeSpeedMultiplier;
        }
    }
}
