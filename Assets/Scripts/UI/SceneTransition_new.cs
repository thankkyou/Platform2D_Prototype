using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneTransition_new : MonoBehaviour
{

    [SerializeField] private string transitionTo;
     [SerializeField] private string targetSpawnID;

    private bool isTransitioning = false;


    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;

           GameManager.Instance.nextSpawnID = targetSpawnID;

            StartCoroutine(Transition());
        }
    }

    IEnumerator Transition()
    {
        yield return ScreenFader.Instance.FadeOutCoroutine();

        SceneManager.LoadScene(transitionTo);
    }

}
