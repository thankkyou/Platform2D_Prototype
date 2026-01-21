using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration = 1f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    //Hiệu ứng fade
    public IEnumerator FadeOut()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / fadeDuration;
            fadeImage.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }
}
