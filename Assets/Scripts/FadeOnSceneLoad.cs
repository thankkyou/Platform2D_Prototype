using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOnSceneLoad : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ScreenFader.Instance != null)
            StartCoroutine(ScreenFader.Instance.FadeInCoroutine());
    }
}
