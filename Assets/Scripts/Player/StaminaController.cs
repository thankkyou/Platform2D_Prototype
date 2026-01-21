using UnityEngine;

public class StaminaController : MonoBehaviour
{
    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaRegenRate = 20f; // hồi mỗi giây
    [SerializeField] private float dashStaminaCost = 50f;

    private float currentStamina;

    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    public delegate void OnStaminaChanged();
    public OnStaminaChanged OnStaminaChangedCallback;

    void Start()
    {
        currentStamina = maxStamina;
        OnStaminaChangedCallback?.Invoke();
    }


    void Update()
    {
        Regenerate();
    }

     void Regenerate()
    {
        if (currentStamina >= maxStamina) return;

        currentStamina += staminaRegenRate * Time.deltaTime;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        OnStaminaChangedCallback?.Invoke();
    }

    public bool CanDash()
    {
        return currentStamina >= dashStaminaCost;
    }

    public void ConsumeDashStamina()
    {
        currentStamina -= dashStaminaCost;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        OnStaminaChangedCallback?.Invoke();
    }

    public void ResetStamina()
    {
        currentStamina = maxStamina;
        OnStaminaChangedCallback?.Invoke();
    }
}
