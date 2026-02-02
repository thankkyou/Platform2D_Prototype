using System.Collections;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1;
    [Space(5)]

    [Header("Vertical Movement Settings:")]
    [SerializeField] private float jumpForce = 45f;
    private float jumpBufferCounter = 0;
    [SerializeField] private float jumpBufferFrames; //ghi nhớ nút nhảy sau vài frame
    private float coyoteTimeCounter = 0; //đếm ngược thời gian nhớ
    [SerializeField] private float coyoteTime; //cho phép nhảy sau khi rời đất
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

    [Header("Dash Through Settings")] //Dash xuyên qua enemy
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
    [Space(5)]

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

    [Header("I-Frame Settings")] //Bất tử sau khi nhận đòn
    [SerializeField] private float iFrameDuration = 0.5f;
    [SerializeField] private int numberOfFlash; //Nhấp nhát khi bất tử
    private Coroutine iFrameCoroutine;
    [Space(5)]

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForceX = 6f;
    [SerializeField] private float knockbackForceY = 2f;
    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockback;
    [Space(5)]

    [Header("Heal Settings")]
    [SerializeField] public int maxHealPotions = 3;
    [SerializeField] private int healAmount = 2;
    [SerializeField] private float healCooldown = 2f;
    [SerializeField] private float healCastTime = 1.2f; 
    [Space(5)]

    public int currentHealPotions;
    private float healCooldownTimer;
    private float healCastTimer;
    private bool isHealingCasting;
    public float HealCooldownTimer => healCooldownTimer; //chỉ đọc
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

    public static System.Action OnPlayerSpawned;


    //Single Skeleton
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
        Debug.Log("Awake: Player new instance created. Health set to: " + Health + " / " + maxHealth);
        OnPlayerSpawned?.Invoke();

    }

    //Lấy tham chiếu
    void Start()
    {
        pState = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        gravity = rb.gravityScale;
        currentHealPotions = maxHealPotions;
        stamina = GetComponent<StaminaController>();

        // Đảm bảo stamina full khi player spawn lần đầu hoặc respawn
        stamina?.ResetStamina();
    }

    //Vẽ hitbox
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
    }

    void Update()
    {
        //Chặn input khi bị knockback
         if (isKnockback)
        {
            UpdateAnimatorParameters();
            return;
        }
        if (pState.alive)
        {
            GetInputs();
        }

        //Chặn input khi dash
        if (pState.dashing) return;

        if (pState.alive)
        {
            UpdateJumpVariables();
            StartDash();
            Flip();
            Move();
            WallSlide();
            Fall();
            Jump();
            Attack();
            Heal();
        }
        UpdateAnimatorParameters();
    }

    private void FixedUpdate()
    {
        if (pState.dashing) return;
    }

    //Đọc input và lưu vào biến
    void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
        attack = Input.GetButtonDown("Attack");
    }

    //Flip nhân vật theo local scale
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

    //Di chuyển 
    private void Move()
    {   
        if (pState.dashing) return;
        // KHÓA DI CHUYỂN KHI BỊ KNOCKBACK
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

        // KHÓA DI CHUYỂN KHI NHẢY TƯỜNG
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
            stamina.ConsumeDashStamina(); //TRỪ STAMINA
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

        rb.gravityScale = 0;    //BỎ QUA TRỌNG LỰC KHI DASH (AIR DASH KHÔNG BỊ RỚT)
        rb.linearVelocity = new Vector2(transform.localScale.x * dashSpeed, 0);

        //BỎ QUA VA CHẠM
        Physics2D.IgnoreLayerCollision(
        LayerMask.NameToLayer("Player"),
        LayerMask.NameToLayer("Attackable"),
        true
        );

        if (Grounded()) Instantiate(dashEffect, transform); //DASH VFX
        yield return new WaitForSeconds(dashTime); //THỜI GIAN DASH

         // TẮT DASH
        rb.gravityScale = gravity; //BẬT LẠI TRỌNG LỰC
        pState.dashing = false;
        pState.invincible = false;

        //BẬT LẠI VA CHẠM
        Physics2D.IgnoreLayerCollision(
            LayerMask.NameToLayer("Player"),
            LayerMask.NameToLayer("Attackable"),
            false
        );

        yield return new WaitForSeconds(dashCooldown); //THỜI GIAN HỒI LẠI DASH
        canDash = true;
    }

    void Attack()
    {
        //KHÓA TẤN CÔNG KHI BỊ KNOCKBACK
        if (isKnockback) return;

        //KHÔNG CHO AIR ATTACK
        if (!Grounded()) return;

        // Giảm timer combo
        if (comboTimer > 0)
            comboTimer -= Time.deltaTime;

        //CHỈ ĐÁNH KHI CÓ INPUT ATTACK
        if (!attack) return;

        //KHÓA DASH KHI TẤN CÔNG
        if (pState.dashing) return;

        // BUFFER ATTACK
        if (isAttacking)
        {
            attackBuffered = true;
            return;
        }
       
        if (comboTimer <= 0)
            comboStep = 0;
            
        comboStep++;

        if (comboStep > 3)
            comboStep = 1;

        // RESET COMBO SAU KHI HẾT THỜI GIAN
        comboTimer = comboResetTime;

        isAttacking = true;
        anim.SetInteger("ComboStep", comboStep);
        anim.SetTrigger("Attack");
        pState.attack = true;
        DoComboHit();
    }

    //HỦY ATK
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
        //CHUYỂN VỂ FALSE ĐỂ NHẬN INPUT MỚI
        pState.attack = false;
        isAttacking = false;

        if (attackBuffered && comboTimer > 0)
        {
            attackBuffered = false;

            comboStep++;
            if (comboStep > 3)
                comboStep = 1; //RESET COMBO

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

    //DAMEMAGE MULTIPLY
    void DoComboHit()
    {
        float finalDamage = damage;

        if (comboStep == 2)
            finalDamage *= 1.2f;
        else if (comboStep == 3)
            finalDamage *= 1.5f;

        Hit(SideAttackTransform, SideAttackArea, finalDamage);
    }

    //Kiểm tra va chạm và gây dame
    private void Hit(Transform _attackTransform, Vector2 _attackArea, float _damage) //TÂM HITBOX, KÍCH THƯỚC, DAMAGE
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(
            _attackTransform.position,
            _attackArea,
            0,
            attackableLayer
        ); //TRẢ VỀ TẤT CẢ COLLIDER NẰM TRONG HITBOX

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            Enemy enemy = objectsToHit[i].GetComponent<Enemy>(); //DUYỆT QUA CÁC COLLIDER CỦA ENEMY
            if (enemy != null)
            {
                enemy.EnemyHit(_damage);
            } //NẾU CÓ COLLIDER GÂY DAME
        }
    }

    //Kiểm tra ground dưới chân
    public bool Grounded()
    {
        return Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround) //Kiểm tra giữa
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround) //Kiểm tra mép trái
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround); //Kiểm tra mép phải
    }
    //Kiểm tra tường bên cạnh
    public bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    //Xử lí nhảy
    void Jump()
    {
        if (isKnockback) return; //Thoát hàm khi bị knockback

        // ATTACK CANCEL KHI NHẢY
        if (isAttacking && Input.GetButtonDown("Jump"))
        {
            CancelAttack();
        }
        
        //Nhảy tường
        if (!Grounded() && IsWalled() && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;
            isWallSliding = false;
            pState.jumping = true;

            float jumpDir = xAxis != 0 //Kiểm tra có input trái/phải
                ? -Mathf.Sign(xAxis) //ngược hướng input
                : -Mathf.Sign(transform.localScale.x); //ngược hướng nhìn

            rb.linearVelocity = Vector2.zero; //reset lại vận tốc
            rb.AddForce(new Vector2(jumpDir * wallJumpForceX, wallJumpForceY), ForceMode2D.Impulse);

            //Flip theo hướng nhảy
            float currentScaleAbsX = Mathf.Abs(transform.localScale.x);
            transform.localScale = new Vector2(jumpDir * currentScaleAbsX, transform.localScale.y);

            //Khóa input tạm thời
            StartCoroutine(WallJumpLock());
            return;
        }

        //Short-hop
        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0 && !Grounded() && !isWallJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            pState.jumping = false;
        }
        //Nhảy thường
        if (!pState.jumping)
        {
            if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                pState.jumping = true;
            }
            else if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump")) //air-jump
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
    //Cập nhật biến phụ trợ cho jump
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
    //trượt tường
    void WallSlide()
    {
        if (!Grounded()
            && IsWalled()
            && rb.linearVelocity.y < 0
            && xAxis != 0 //đang giữ phím trái phải
            && Mathf.Sign(xAxis) == Mathf.Sign(transform.localScale.x) //hướng input trùng với hướng quay mặt
            && !pState.dashing)
        {
            isWallSliding = true;

            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x, //giữ vận tốc trục x
                Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue) //duy trì tốc độ không quá wallslidespeed
            );
        }
        else
        {
            isWallSliding = false;
        }

        anim.SetBool("WallSliding", isWallSliding);
    }
    //Animation rơi
    void Fall()
    {
        bool isFalling =
            !Grounded() &&
            rb.linearVelocity.y < -0.1f &&
            !isWallSliding &&
            !pState.dashing;

        anim.SetBool("Falling", isFalling);
    }

    //Đồng bộ trạng thái vật lí với animator
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

    //bất tử sau khí nhận dame
    IEnumerator iFrames()
    {
        Physics2D.IgnoreLayerCollision(10, 8, true); //bỏ qua va chạm giữa layer 8, 10
        //Iframe duration
        for (int i = 0; i < numberOfFlash; i++) //Nháy sprite
        {
            sr.color = new Color(1, 0, 0, 0.5f); 
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlash));
            sr.color = Color.white;
            yield return new WaitForSeconds(iFrameDuration / (numberOfFlash));
        }
        Physics2D.IgnoreLayerCollision(10, 8, false);

        // pState.invincible = true;
        // anim.SetTrigger("TakeDamage");
        // yield return new WaitForSeconds(1f);
        // pState.invincible = false;
    }

    //Xử lí khi nhân vật bị đánh
    public void TakeDamage(float _damage)
    {
        if (pState.alive)
        {
            Health -= Mathf.RoundToInt(_damage);
            if (Health <= 0)
            {
                Health = 0;
                Die();
            }
            else
            {
                anim.SetTrigger("TakeDamage");
                ApplyKnockback();
                StartCoroutine(iFrames());
            }
        }
        if (pState.dashing || isDead || isKnockback) return; //không nhận dame

        if (isHealingCasting) //đang heal mà nhận dame sẽ thoát heal
            CancelHeal();

        // Trừ máu 
        // Health -= Mathf.RoundToInt(_damage);

        
        // anim.SetTrigger("TakeDamage");
        // ApplyKnockback();
        // StartCoroutine(iFrames());

        // Debug.Log("TakeDamage: Health reduced to " + Health);
    }
    

    void Die()
    {
        if (isDead) return;
        pState.alive = false;
        isDead = true;

        // Dừng mọi trạng thái
        StopAllCoroutines();
        rb.linearVelocity = Vector2.zero;

        pState.attack = false;
        pState.dashing = false;
        pState.jumping = false;
        pState.invincible = false;

        anim.ResetTrigger("TakeDamage");
        anim.SetTrigger("Death");

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Attackable"),true);

        stamina?.ResetStamina(); //Không null => reset stamina
        StartCoroutine(UIManager.Instance.ActivateDeathScreen());
        // Invoke(nameof(Respawn), 1.5f);
        Debug.Log("Die: Health = " + Health + " → Respawn invoked");
    }

    // IEnumerator Death()
    // {
    //     pState.alive = false;
    //     Time.timeScale = 1;
    //     anim.SetTrigger ("Death");

    //     yield return new WaitForSeconds(0.9f);
    //     Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"),LayerMask.NameToLayer("Attackable"),true);
    //     StartCoroutine(UIManager.Instance.ActivateDeathScreen());
    // }

    public void Respawned()
    {
        isDead = false;
        // rb.linearVelocity = Vector2.zero;
        // SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        if (!pState.alive)
        {
            pState.alive = true;
            Health = maxHealth;
            currentHealPotions = maxHealPotions;
            ResetHeal();
            anim.Play("player_idle");
        }
    }

    //Health property
    public int Health
    {
        get { return health; }
        set {
            if (health != value) //Health khác giá trị cũ
            {
                health = Mathf.Clamp(value, 0, maxHealth);
                Debug.Log($"Health changed to {health}/{maxHealth}");

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

            //Cast xong
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

    public void ResetHeal()
    {
        healCooldownTimer = 0f;
        healCastTimer = 0f;
        isHealingCasting = false;
        pState.healing = false;

        anim.SetBool("Healing", false);
    }

    public void FullRestore()
    {
        Health = maxHealth;
        currentHealPotions = maxHealPotions;

        ResetHeal();

        stamina?.ResetStamina();
    }


    //hitstop
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
        if (isKnockback) return; //chặn knockback chồng

        isKnockback = true;

        // hướng bị đánh
        float dir = pState.lookingRight ? -1f : 1f;

        rb.linearVelocity = Vector2.zero; //reset vận tốc
        rb.AddForce(
            new Vector2(dir * knockbackForceX, knockbackForceY),
            ForceMode2D.Impulse
        ); //đẩy

        anim.SetTrigger("TakeDamage");

        StartCoroutine(KnockbackRoutine()); //chờ knockback xong
    }

    IEnumerator KnockbackRoutine()
    {
        yield return new WaitForSeconds(knockbackDuration);
        isKnockback = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Đặt vị trí spawn tại checkpoint đã lưu
        SpawnPoint[] points = FindObjectsOfType<SpawnPoint>();
        foreach (var point in points)
        {
            if (point.spawnID == GameManager.Instance.nextSpawnID)
            {
                transform.position = point.transform.position;
                break;
            }
        }
        Health = maxHealth;
        currentHealPotions = maxHealPotions;
        stamina?.ResetStamina();
        ForceUpdateHealthUI();
    }

    // void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     // Respawn theo SaveData
    //     if (!string.IsNullOrEmpty(SaveData.Instance.checkpointScene))
    //     {
    //         transform.position = SaveData.Instance.checkpointPos;
    //     }

    //     Health = maxHealth;
    //     currentHealPotions = maxHealPotions;
    //     stamina?.ResetStamina();

    //     ForceUpdateHealthUI();
    // }


    public void ForceUpdateHealthUI()
    {
        OnHealthChangedCallback?.Invoke();
    }
}