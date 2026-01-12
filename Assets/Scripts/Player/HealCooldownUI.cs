using UnityEngine;
using UnityEngine.UI;

public class HealCooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    PlayerController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = PlayerController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCooldown();
    }

    void UpdateCooldown()
    {
        if (PlayerController.Instance == null) return;
        cooldownImage.color =
        player.HealCooldownTimer > 0
        ? new Color(1,1,1,0.4f)
        : Color.white;

        if (player.HealCooldownTimer <= 0)
        {
            cooldownImage.fillAmount = 1;
        }
        else
        {
            cooldownImage.fillAmount =
                1 - (player.HealCooldownTimer / player.HealCooldown);
        }
    }
}