using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField]protected PlayerController player;
    [SerializeField]protected float speed;
    [SerializeField]protected float damage;
    [SerializeField] protected GameObject blood;

    [Header("Knockback:")]
    [SerializeField] protected float knockbackForce = 5f;
    [SerializeField] protected float knockbackDuration = 0.2f;
    protected bool isKnockback = false;
    [Space(5)]

    [Header("Stun Settings")]
    [SerializeField] protected float stunDuration = 0.5f;
    protected bool isStunned = false;
    [Space(5)]

    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
    AudioManager audioManager;
    protected enum EnemyStates
    {
        //Mộc Tinh
        MocTinh_Idle,
        MocTinh_flip,

        //Ma Da
        MaDa_Idle,
        MaDa_Chase,
        MaDa_Stunned,
        MaDa_Death,

        //Quy nhap trang
        QuyNhapTrang_Idle,
        QuyNhapTrang_Suprised,
        QUyNhapTrang_Charge,
    }
    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set 
        { 
            if (currentEnemyState != value)
            {
                currentEnemyState = value;

                ChangeCurrentAnimation();
            }
        }
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateEnemyStates();
        if (GameManager.Instance.gameIsPaused) return;
    }

    protected void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
        {
            Attack();
            PlayerController.Instance.HitStop(0, 0.2f);
        }
    }

    protected virtual void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }
    }

    protected virtual void ChangeCurrentAnimation(){}

    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }

    public virtual void EnemyHit(float _damageDone)
    {
        health -= (int)_damageDone;

        Vector3 bloodPos = transform.position + new Vector3(0, 1, 0);
        GameObject _blood = Instantiate(blood, bloodPos, Quaternion.identity);

        SpriteRenderer bloodSr = _blood.GetComponent<SpriteRenderer>();

        audioManager.PlaySFX(audioManager.enemyHit);

        if (bloodSr != null)
        {
            float dir = transform.position.x > PlayerController.Instance.transform.position.x ? 1f : -1f;
            bloodSr.flipX = dir < 0;
        }

        Destroy(_blood, 1f);

        // if (!isKnockback)
        //     StartCoroutine(KnockbackRoutine());
        
        if (!isStunned)
            StartCoroutine(StunRoutine());

        
        StartCoroutine(HitFlash());
    }

    IEnumerator StunRoutine()
    {
        anim.enabled = false;
        isStunned = true;
        yield return new WaitForSeconds(stunDuration);
        isStunned = false;
        anim.enabled = true;
    }

    IEnumerator HitFlash()
    {

        sr.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        sr.color = Color.white;
    }

    IEnumerator KnockbackRoutine()
    {
        isKnockback = true;

        // Hướng bị đẩy = ngược hướng player
        float dir = transform.position.x > PlayerController.Instance.transform.position.x ? 1f : -1f;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(new Vector2(dir * knockbackForce, 2f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockback = false;
    }

    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}
