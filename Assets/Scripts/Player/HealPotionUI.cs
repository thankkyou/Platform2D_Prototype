using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealPotionUI : MonoBehaviour
{
     [SerializeField] private TextMeshProUGUI potionText;
    PlayerController player;

    void Start()
    {
        player = PlayerController.Instance;
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (PlayerController.Instance == null) return;
        potionText.text = player.currentHealPotions.ToString();
    }
}
