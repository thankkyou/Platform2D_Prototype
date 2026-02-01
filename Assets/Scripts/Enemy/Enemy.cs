using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField]protected PlayerController player;
    [SerializeField]protected float speed;
    [SerializeField]protected float damage;
    [SerializeField] protected GameObject blood;
    protected Rigidbody2D rb;
    protected SpriteRenderer sr;
    protected Animator anim;
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
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateEnemyStates();
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
        GameObject _blood = Instantiate(blood, transform.position, Quaternion.identity);
        Destroy(_blood, 5.5f);

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
