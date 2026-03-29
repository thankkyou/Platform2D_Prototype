using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using Unity.Mathematics;
using System;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;
    [SerializeField] private float fadeTime;
    private Image fadeOutUIImage;


    void Start()
    {
        fadeOutUIImage = GetComponent<Image>();
        
        // Tự động mờ sáng (Fade In) mỗi khi bắt đầu một Scene
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            StartCoroutine(FadeInCoroutine());
        }
    }
    void Update()
    {
        
    }
    public enum FadeDirection
    {
        In,
        Out,
    }

    public void CallFadeAndLoadScene(string _sceneToLoad)
    {
        StartCoroutine(FadeAndLoadScene(FadeDirection.In, _sceneToLoad));
    }

    public IEnumerator Fade(FadeDirection _fadeDireection)
    {
        float _alpha = _fadeDireection == FadeDirection.Out ? 1 : 0;
        float _fadeEndValue = _fadeDireection == FadeDirection.Out ? 0 : 1;
        if (_fadeDireection == FadeDirection.Out)
        {
            while (_alpha >= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDireection);
                yield return null;
            }
            fadeOutUIImage.enabled = false;
        }
        else
        {
            fadeOutUIImage.enabled = true;
            while (_alpha <= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDireection);
                yield return null;
            }
        }
    }

    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDireection, string _sceneToLoad)
    {
        fadeOutUIImage.enabled= true;
        yield return Fade(_fadeDireection);
        SceneManager.LoadScene(_sceneToLoad);
    }
    
    void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
    {
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, _alpha);

        _alpha += Time.unscaledDeltaTime * (1/fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }

    public void LoadSceneImmediately(string _sceneToLoad)
    {
        Time.timeScale = 1f; // Đảm bảo game tiếp tục chạy (unpause)
        SceneManager.LoadScene(_sceneToLoad);
    }



    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float fadeDuration = 0.5f;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator FadeOutCoroutine()
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
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
            t += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
    }
    
}
