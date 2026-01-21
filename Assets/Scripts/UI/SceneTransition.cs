using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private float delay = 0f;

    private bool triggered;

    //Xử lí va chạn người chơi
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(LoadScene());
        }
    }

    //Đổi màn
    private IEnumerator LoadScene()
    {
        yield return ScreenFader.Instance.FadeOut();
        SceneManager.LoadScene(sceneToLoad);
    }

}
