using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    [SerializeField]private float projectileCooldown;
    [SerializeField]private Transform arrowPoints;
    [SerializeField]private GameObject arrowPrefab;
    private Animator anim;
    private PlayerController playerController;
    private float cooldownTimer = Mathf.Infinity;
    private PlayerStateList pState;

    AudioManager audioManager;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        pState = GetComponent<PlayerStateList>();
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

    }

    private void Update()
    {
        Fire();
        cooldownTimer += Time.deltaTime;
    }

    private void Fire()
    {
        if(Input.GetButtonDown("Fire") && playerController.isGround && cooldownTimer > projectileCooldown)
        {
            Projectile();
        }
    }

    private void Projectile()
    {
        StaminaController stamina = playerController.GetComponent<StaminaController>();
        if (stamina == null || !stamina.CanShoot()) return;

        stamina.ConsumeProjectileStamina();
        pState.firing = true;
        anim.SetTrigger("Projectile");
        cooldownTimer = 0;
    }

    public void SpawnArrow()
    {
        GameObject arrow = Instantiate(arrowPrefab, arrowPoints.position, Quaternion.identity);
        float direction = Mathf.Sign(transform.localScale.x);
        arrow.GetComponent<ProjectileObject>().SetDirection(direction);
        audioManager.PlaySFX(audioManager.playerShoot);
    }

    public void OnFireAnimationEnd()
    {
        pState.firing = false;
    }

}
