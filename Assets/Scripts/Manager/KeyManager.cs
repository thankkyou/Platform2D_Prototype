using UnityEngine;
using TMPro;
using System.Collections;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;

    [SerializeField] private int requiredKeys = 3;
    [SerializeField] private TextMeshProUGUI keyText;

    [SerializeField]private int collectedKeys = 0;

    public int CollectedKeys => collectedKeys;
    public int RequiredKeys => requiredKeys;
    public bool HasAllKeys => collectedKeys >= requiredKeys;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void CollectKey()
    {
        collectedKeys = Mathf.Min(collectedKeys + 1, requiredKeys);
        UpdateUI();
    }

    void UpdateUI()
    {
        if (keyText != null)
            keyText.text = $"Chìa khóa đã thu thập {collectedKeys}/{requiredKeys}";
    }

    public void FlashNotEnoughKeys()
    {
        StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        Debug.Log("FlashRed started");
        keyText.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        keyText.color = Color.white;
        yield return new WaitForSeconds(0.2f);
    }
}