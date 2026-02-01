using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public ScreenFader screenFader;
    public static UIManager Instance;
    [SerializeField] GameObject deathScreen;

    void Awake() 
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
        screenFader = GetComponent<ScreenFader>();

    }

    public IEnumerator ActivateDeathScreen()
    {
        // yield return new WaitForSeconds(0.8f);
        // StartCoroutine(screenFader.FadeInCoroutine());

        yield return new WaitForSeconds(.8f);
        deathScreen.SetActive(true);
    }

    public IEnumerator DeactivateDeathScreen()
    {
        yield return new WaitForSeconds(0.5f);
        deathScreen.SetActive(false);
        // StartCoroutine(screenFader.FadeOutCoroutine());
    }
}
