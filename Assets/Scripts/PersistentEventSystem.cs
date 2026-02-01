using UnityEngine;

public class PersistentEventSystem : MonoBehaviour
{
    public static PersistentEventSystem Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
