using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private Image staminaFill;

    private StaminaController stamina;

    void Start()
    {
        stamina = FindObjectOfType<StaminaController>();

        if (stamina != null)
        {
            stamina.OnStaminaChangedCallback += UpdateStaminaUI;
            UpdateStaminaUI(); // update lần đầu
        }
    }

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
