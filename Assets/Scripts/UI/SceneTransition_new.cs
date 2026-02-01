using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneTransition_new : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private string transitionTo;
    // [SerializeField] private Transform startPoint;
     [SerializeField] private string targetSpawnID;

    private bool isTransitioning = false;

    // [SerializeField] private Vector2 exitDirection;
    // [SerializeField] private float exitTime;

    // private void Start()
    // {
    //     if (transitionTo == GameManager.Instance.transitionedFromScene)
    //     {
    //         PlayerController.Instance.transform.position = startPoint.position;
    //     }
    // }

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

    // IEnumerator TransitionCoroutine()
    // {
    //     // Lưu scene cũ
    //     GameManager.Instance.transitionedFromScene =
    //         SceneManager.GetActiveScene().name;

    //     // Fade Out
    //     yield return ScreenFader.Instance.FadeOutCoroutine();

    //     // Load scene mới
    //     SceneManager.LoadScene(transitionTo);
    // }

}
