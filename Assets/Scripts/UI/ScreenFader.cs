using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using Unity.Mathematics;
using System;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float fadeTime;
    private Image fadeOutUIImage;

    
    void Start()
    {
        fadeOutUIImage = GetComponent<Image>();
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

        _alpha += Time.deltaTime * (1/fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }
    
}
