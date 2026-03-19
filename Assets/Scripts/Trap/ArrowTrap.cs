using UnityEngine;

public class ArrowTrap : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private float detectionRange = 10f;
    private float cooldownTimer;

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= attackCooldown && PlayerInRange())
        {
            Attack();
        }
    }

    bool PlayerInRange()
    {
        if (PlayerController.Instance == null) return false;
        return Vector2.Distance(transform.position, PlayerController.Instance.transform.position) <= detectionRange;
    }

    private void Attack()
    {
        cooldownTimer = 0;

        GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
        ProjectileObject proj = arrow.GetComponent<ProjectileObject>();

        if (proj != null)
        {
            // Hướng bắn dựa vào vị trí player so với trap
            float dir = PlayerController.Instance.transform.position.x > transform.position.x ? 1f : -1f;
            proj.SetDirection(dir);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}