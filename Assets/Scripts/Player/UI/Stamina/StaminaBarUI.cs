using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private Image staminaFill;

    private StaminaController stamina;

    void Start()
    {
        stamina = FindFirstObjectByType<StaminaController>();

        if (stamina != null)
        {
            stamina.OnStaminaChangedCallback += UpdateStaminaUI;
            UpdateStaminaUI(); // update lần đầu
        }
    }

    // void OnEnable()
    // {
    //     PlayerController.OnPlayerSpawned += Setup;
    // }

    // void OnDisable()
    // {
    //     PlayerController.OnPlayerSpawned -= Setup;
    // }

    // void Setup()
    // {
    //     if (stamina != null)
    //         stamina.OnStaminaChangedCallback -= UpdateStaminaUI;

    //     stamina = FindFirstObjectByType<StaminaController>();

    //     if (stamina != null)
    //     {
    //         stamina.OnStaminaChangedCallback += UpdateStaminaUI;
    //         UpdateStaminaUI();
    //     }
    // }


    void UpdateStaminaUI()
    {
        float percent = stamina.CurrentStamina / stamina.MaxStamina;
        staminaFill.fillAmount = percent;
    }

    private void OnDestroy()
    {
        if (stamina != null)
        {
            stamina.OnStaminaChangedCallback -= UpdateStaminaUI;
        }
    }
}
