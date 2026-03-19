using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class SettingMenu : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;
    public void SetVolume(float _volume)
    {
        audioMixer.SetFloat("Volume", _volume);
    }

    public void SetQuality(int _qualityInxdex)
    {
        QualitySettings.SetQualityLevel(_qualityInxdex);
    }

    public void SetFullScreen(bool _isFullscreen)
    {
        Screen.fullScreen = _isFullscreen;
    }

    public void Quit()
    {
        Application.Quit();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
