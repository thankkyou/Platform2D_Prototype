using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public ScreenFader screenFader;
    public static UIManager Instance;
    [SerializeField] GameObject deathScreen;

    void Awake() 
    {
        Instance = this;
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
