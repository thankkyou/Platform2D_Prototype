using UnityEngine;

public class AutoParallax : MonoBehaviour
{
    public float scrollSpeed = 2f;

    private float length;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float newPos = Mathf.Repeat(Time.time * scrollSpeed, length);
        transform.position = startPos + Vector3.left * newPos;
    }
}
