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
        StartCoroutine(ScreenFader.Instance.FadeInCoroutine());
    }
}
