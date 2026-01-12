using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings:")]
    [SerializeField] private float jumpForce = 45f;
    private float jumpBufferCounter = 0;
    [SerializeField] private float jumpBufferFrames;
    private float coyoteTimeCounter = 0;
    [SerializeField] private float coyoteTime;
    private int airJumpCounter = 0;
    [SerializeField] private int maxAirJumps;
    [Space(5)]

    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckY = 0.2f;
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] public LayerMask whatIsGround;
    [Space(5)]

    [Header("Dash Settings:")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] GameObject dashEffect;
    [Space(5)]

    [Header("Dash Through Settings")]
    [SerializeField] private LayerMask enemyLayer;
    [Space(5)]

    [Header("Wall Settings:")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallJumpForceX = 12f;
    [SerializeField] private float wallJumpForceY = 40f;
    [SerializeField] private float wallJumpLockTime = 0.15f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [Space(5)]

    [Header("Attacking:")]
    [SerializeField] Transform SideAttackTransform;
    [SerializeField] Vector2 SideAttackArea;
    [SerializeField] LayerMask attackableLayer;
    private bool attack = false;
    private float damage = 2;

    bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    [Header("Combo Attack Settings")]
    [SerializeField] private float comboResetTime = 0.6f;
    private int comboStep = 0;
    private float comboTimer;
    private bool attackBuffered = false;
    private bool isAttacking = false;
    [Space(5)]

    [Header("Health Settings:")]
    public int health;
    public int maxHealth;
    private bool isDead;
    [SerializeField] float hitFlashSpeed;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate OnHealthChangedCallback;
    [Space(5)]

    [Header("I-Frame Settings")]
    [SerializeField] private float iFrameDuration = 0.5f;
    [SerializeField] private int numberOfFlash;
    private Coroutine iFrameCoroutine;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForceX = 6f;
    [SerializeField] private float knockbackForceY = 2f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockback;

    [Header("Heal Settings")]
    [SerializeField] private int maxHealPotions = 3;
    [SerializeField] private int healAmount = 2;
    [SerializeField] private float healCooldown = 2f;
    [SerializeField] private float healCastTime = 1.2f; // ⏳ thời gian cast

    public int currentHealPotions;
    private float healCooldownTimer;
    private float healCastTimer;
    private bool isHealingCasting;
    public float HealCooldownTimer => healCooldownTimer;
    public float HealCooldown => healCooldown;


    [HideInInspector]public PlayerStateList pState;
    private Rigidbody2D rb;
    Animator anim;
    private SpriteRenderer sr;
    private StaminaController stamina;


    private float xAxis, yAxis;
    private float gravity;

    private bool canDash = true;
    private bool dashed;
    private bool isGround;
    private float velocityY;
    private bool isWallJumping;
    private bool isWallSliding;

    public static PlayerController Instance;
    private void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        Health = maxHealth;
    }

    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        currentHealPotions = maxHealPotions;
        stamina = GetComponent<StaminaController>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }

    void Update()
    {
         if (isKnockback)
        {
            UpdateAnimatorParameters();
            return;
        }

        GetInputs();
        UpdateJumpVariables();
        StartDash();
        if (pState.dashing) return;
        Flip();
        Move();
        WallSlide();
        Fall();
        Jump();
        UpdateAnimatorParameters();
        Attack();
        Heal();
    }

    private void FixedUpdate()
    {
        if (pState.dashing) return ;
    }

    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
    }

    void Flip()
    {
        if (isAttacking) return;

        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            pState.lookingRight = true;
        }
    }

    private void Move()
    {   

        if (isKnockback) return;

        // KHÓA DI CHUYỂN KHI ĐANG HEAL
        if (isHealingCasting)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }


        // KHÓA DI CHUYỂN KHI ĐANG ATTACK
        if (isAttacking) 
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // KHÓA DI CHUYỂN KHI CHẾT
        if (isDead) return;

        if (isWallJumping) return;

        rb.linearVelocity = new Vector2(walkSpeed * xAxis, rb.linearVelocity.y);
        anim.SetBool("Walking", rb.linearVelocity.x != 0 && Grounded());
    }

    void StartDash()
    {
        if (isKnockback) return;
        if (Input.GetButtonDown("Dash")
        && canDash
        && !dashed
        && stamina != null
        && stamina.CanDash())
        {
            stamina.ConsumeDashStamina(); // TRỪ STAMINA
            StartCoroutine(Dash());
            dashed = true;
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        pState.invincible = true;

        anim.SetTrigger("Dashing");

        rb.gravityScale = 0;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);

        Physics2D.IgnoreLayerCollision(
        LayerMask.NameToLayer("Player"),
        LayerMask.NameToLayer("Attackable"),
        true
        );

        if (Grounded()) Instantiate(dashEffect, transform);
        yield return new WaitForSeconds(dashTime);

         // TẮT DASH
        rb.gravityScale = gravity;
        pState.dashing = false;
        pState.invincible = false;

        // BẬT LẠI VA CHẠM
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Attackable"),
            false
        );

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void Attack()
    {
        if (isKnockback) return;

        if (!Grounded()) return;

        // Giảm timer combo
        if (comboTimer > 0)
            comboTimer -= Time.deltaTime;

        if (!attack) return;
        if (pState.dashing) return;

        // Nếu đang đánh → buffer input
        if (isAttacking)
        {
            attackBuffered = true;
            return;
        }

        // Nếu hết thời gian combo → reset trước khi đánh
        if (comboTimer <= 0)
            comboStep = 0;

        comboStep++;
        if (comboStep > 3)
            comboStep = 1;

        comboTimer = comboResetTime;

        isAttacking = true;
        anim.SetInteger("ComboStep", comboStep);
        anim.SetTrigger("Attack");
        pState.attack = true;
        DoComboHit();
    }

    void CancelAttack()
    {
        isAttacking = false;
        attackBuffered = false;
        pState.attack = false;

        comboStep = 0;
        comboTimer = 0;

        anim.ResetTrigger("Attack");
        anim.SetInteger("ComboStep", 0);
    }



    // Gọi từ Animation Event ở frame cuối mỗi animation attack
    public void CheckForBufferedAttack()
    {   
        pState.attack = false;
        isAttacking = false;

        if (attackBuffered && comboTimer > 0)
        {
            attackBuffered = false;

            comboStep++;
            if (comboStep > 3)
                comboStep = 1;

            comboTimer = comboResetTime;

            pState.attack = true;
            isAttacking = true;
            anim.SetInteger("ComboStep", comboStep);
            anim.SetTrigger("Attack");

            DoComboHit();
        }
        else
        {
            ResetCombo();
        }
    }

    void DoComboHit()
    {
        float finalDamage = damage;

        if (comboStep == 2)
            finalDamage *= 1.2f;
        else if (comboStep == 3)
            finalDamage *= 1.5f;

        Hit(SideAttackTransform, SideAttackArea, finalDamage);
    }

    private void Hit(Transform _attackTransform, Vector2 _attackArea, float _damage)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(
            _attackTransform.position,
            _attackArea,
            0,
            attackableLayer
        );

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy enemy = objectsToHit[i].GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.EnemyHit(_damage);
            }
        }
    }

    public bool Grounded()
    {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround);
    }

    public bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    void Jump()
    {
        if (isKnockback) return;

        // ATTACK CANCEL KHI NHẢY
        if (isAttacking && Input.GetButtonDown("Jump"))
        {
            CancelAttack();
        }
        
        if (!Grounded() && IsWalled() && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;
            isWallSliding = false;
            pState.jumping = true;

            float jumpDir = xAxis != 0 ? -Mathf.Sign(xAxis) : -Mathf.Sign(transform.localScale.x);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(new Vector2(jumpDir * wallJumpForceX, wallJumpForceY), ForceMode2D.Impulse);

            float currentScaleAbsX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector2(jumpDir * currentScaleAbsX, transform.localScale.y);

            StartCoroutine(WallJumpLock());
            return;
        }

        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0 && !Grounded() && !isWallJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            pState.jumping = false;
        }

        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
            {
                pState.jumping = true;
                airJumpCounter++;
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }

        anim.SetBool("Jumping", rb.linearVelocity.y > 0 && !Grounded());
    }

    IEnumerator WallJumpLock()
    {
        yield return new WaitForSeconds(wallJumpLockTime);
        isWallJumping = false;
    }

    void UpdateJumpVariables()
    {
        if (Grounded())
        {
            pState.jumping = false;
            coyoteTimeCounter = coyoteTime;
            airJumpCounter = 0;
            isWallJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames;
        }
        else
        {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10;
        }
        if (Grounded())
        {
            anim.SetBool("Falling", false);
            anim.SetBool("WallSliding", false);
        }
    }

    void WallSlide()
    {
        if (!Grounded()
            && IsWalled()
            && rb.linearVelocity.y < 0
            && xAxis != 0
            && Mathf.Sign(xAxis) == Mathf.Sign(transform.localScale.x)
            && !pState.dashing)
        {
            isWallSliding = true;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue)
            );
        }
        else
        {
            isWallSliding = false;
        }

        anim.SetBool("WallSliding", isWallSliding);
    }

    void Fall()
    {
        bool isFalling =
            !Grounded() &&
            rb.linearVelocity.y < -0.1f &&
            !isWallSliding &&
            !pState.dashing;

        anim.SetBool("Falling", isFalling);
    }

    void UpdateAnimatorParameters()
    {
        isGround = Grounded();
        velocityY = rb.linearVelocity.y;

        anim.SetBool("isGround", isGround);
        anim.SetFloat("velocityY", velocityY);
    }

    public void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0;
        attackBuffered = false;
        isAttacking = false;
        pState.attack = false;

        anim.SetInteger("ComboStep", 0);
    }

    IEnumerator iFrames()
    {
        Physics2D.IgnoreLayerCollision(10, 8, true);
        //Iframe duration
        for (int i = 0; i < numberOfFlash; i++)
        {
            sr.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlash));
            sr.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlash));
        }
        Physics2D.IgnoreLayerCollision(10, 8, false);
    }

    public void TakeDamage(float _damage)
    {
        if (pState.dashing || isDead || isKnockback) return;

        if (isHealingCasting)
            CancelHeal();

        // Trừ máu trước
        Health -= Mathf.RoundToInt(_damage);

        if (Health <= 0)
        {
            Die();
            return;
        }
        anim.SetTrigger("TakeDamage");
        ApplyKnockback();
        StartCoroutine(iFrames());
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // Dừng mọi trạng thái
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;

        pState.attack = false;
        pState.dashing = false;
        pState.jumping = false;
        pState.invincible = false;

        anim.ResetTrigger("TakeDamage");
        anim.SetTrigger("Die");

        stamina?.ResetStamina();
    }

    public int Health
    {
        get { return health; }
        set {
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth);

                if (OnHealthChangedCallback != null)
                {
                    OnHealthChangedCallback.Invoke();
                }
            }
        }
    }

    void Heal()
    {
        // Cooldown
        if (healCooldownTimer > 0)
            healCooldownTimer -= Time.deltaTime;

        // Nếu đang cast heal
        if (isHealingCasting)
        {
            healCastTimer += Time.deltaTime;

            // Cancel nếu làm hành động khác
            if (Mathf.Abs(rb.linearVelocity.x) > 0.01f
                || Input.GetButtonDown("Jump")
                || Input.GetButtonDown("Dash")
                || Input.GetButtonDown("Attack")
                || pState.dashing
                || pState.jumping
                || pState.invincible)
            {
                CancelHeal();
                return;
            }

            // ✅ Cast xong
            if (healCastTimer >= healCastTime)
            {
                FinishHeal();
            }

            return;
        }

        // Bắt đầu cast heal
        if (Input.GetKey("f")
            && Health < maxHealth
            && currentHealPotions > 0
            && healCooldownTimer <= 0
            && !pState.dashing
            && !pState.jumping
            && !isAttacking
            && Mathf.Abs(rb.linearVelocity.x) < 0.01f)
        {
            StartHealCast();
        }
    }

    void StartHealCast()
    {
        // Trừ bình
        currentHealPotions--;

        isHealingCasting = true;
        healCastTimer = 0;
        pState.healing = true;

        rb.linearVelocity = Vector2.zero;
        anim.SetBool("Healing", true);
    }

    void FinishHeal()
    {
        isHealingCasting = false;
        pState.healing = false;

        // Hồi máu
        Health += healAmount;
        Health = Mathf.Clamp(Health, 0, maxHealth);

        // Cooldown
        healCooldownTimer = healCooldown;

        anim.SetBool("Healing", false);
    }

    void CancelHeal()
    {
        isHealingCasting = false;
        pState.healing = false;
        healCastTimer = 0;

        anim.SetBool("Healing", false);
    }

    public void HitStop(float timeScale, float duration)
    {
        StartCoroutine(HitStopCoroutine(timeScale, duration));
    }

    IEnumerator HitStopCoroutine(float timeScale, float duration)
    {
        float originalScale = Time.timeScale;
        Time.timeScale = timeScale;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalScale;
    }

    void ApplyKnockback()
    {
        if (isKnockback) return;

        isKnockback = true;

        // hướng bị đánh
        float dir = pState.lookingRight ? -1f : 1f;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(
            new Vector2(dir * knockbackForceX, knockbackForceY),
            ForceMode2D.Impulse
        );

        anim.SetTrigger("TakeDamage");

        StartCoroutine(KnockbackRoutine());
    }

    IEnumerator KnockbackRoutine()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }

    //Kiểm tra Animation "Die"
    public delegate void OnPlayerDeath();
    public event OnPlayerDeath OnDeathAnimationFinished;
    public void DieAnimationFinished()
    {
        OnDeathAnimationFinished?.Invoke();
    }


}