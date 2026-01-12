using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;

    [SerializeField]protected PlayerController player;
    [SerializeField]protected float speed;

    [SerializeField]protected float damage;
    protected Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
    public virtual void EnemyHit(float _damageDone)
    {
        health -= _damageDone;

    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
            PlayerController.Instance.HitStop(0, 0.2f);
        }
    }

    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}
