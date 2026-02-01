using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using Unity.Mathematics;
using System;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    // [SerializeField] Image fadeImage;
    // [SerializeField] float fadeDuration = 1f;

    // void Awake()
    // {
    //     if (Instance == null)
    //         Instance = this;
    // }

    // //Hiệu ứng fade
    // public IEnumerator FadeOut()
    // {
    //     float t = 0;
    //     while (t < 1)
    //     {
    //         t += Time.deltaTime / fadeDuration;
    //         fadeImage.color = new Color(0, 0, 0, t);
    //         yield return null;
    //     }
    // }


    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public IEnumerator FadeOutCoroutine()
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1;
    }

    public IEnumerator FadeInCoroutine()
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }
}
