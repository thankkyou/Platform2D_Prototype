using UnityEngine;

public class ProjectileObject : MonoBehaviour
{
    [SerializeField] private float speed = 50f;
    [SerializeField] private float damage = 1f;
    private float direction;
    private bool hit;

    private BoxCollider2D boxCollider;
    // private Animator anim;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        // anim = GetComponent<Animator>();

    }

    private void Start()
    {
        Destroy(gameObject, 5f); // Tự hủy sau 5 giây nếu không trúng gì
    }

    private void Update()
    {
        if (hit) return;
        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;

        // Gây sát thương nếu trúng enemy
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.EnemyHit(damage);
        }

    Destroy(gameObject);
    }

    public void SetDirection(float _direction)
    {
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

}
