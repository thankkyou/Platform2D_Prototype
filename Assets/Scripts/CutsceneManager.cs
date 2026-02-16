using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutsceneManager : MonoBehaviour
{
    [Header("Video")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName;

    private bool isSkipped = false;

    private void Start()
    {
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    // Gọi khi video chạy hết
    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    // Gắn cho nút Skip
    public void SkipCutscene()
    {
        if (isSkipped) return;
        isSkipped = true;

        videoPlayer.Stop();
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
