using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField]private float projectileCooldown;
    private Animator anim;
    private PlayerStateList pState;
    private float cooldownTimer = Mathf.Infinity;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        pState = GetComponent<PlayerStateList>();
    }

    private void Update()
    {
        if(Input.GetMouseButton(1) && cooldownTimer > projectileCooldown && pState.attack)
        {
            Projectile();
        }

        cooldownTimer += Time.deltaTime;
    }

    private void Projectile()
    {
        anim.SetTrigger("Projectile");
        cooldownTimer = 0;
    }
}
