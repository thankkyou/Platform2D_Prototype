using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public ScreenFader screenFader;
    public static UIManager Instance;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject victoryScreen; // Màn hình chiến thắng

    void Awake() 
    {
        Instance = this;
        screenFader = GetComponent<ScreenFader>();
    }

    public void ActivateVictoryScreen()
    {
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
            Time.timeScale = 0f; // Dừng thời gian (game pause)
        }
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
