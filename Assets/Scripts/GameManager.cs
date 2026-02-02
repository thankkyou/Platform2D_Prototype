
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;

    public string nextSpawnID;

    // public int savedHealth;
    // public int savedPotions;

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] CheckPoint checkPoint;

    public static GameManager Instance { get; private set;}

    public void SetCheckpoint(CheckPoint cp)
    {
        checkPoint = cp;
        Debug.Log("GameManager saved checkpoint: " + cp.transform.position);
    }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }



    public void RespawnPlayer()
    {
        if (checkPoint != null)
        {
            if (checkPoint.interacted)
            {
                respawnPoint = checkPoint.transform.position;
            }
            else
            {
                respawnPoint = platformingRespawnPoint;
            }
        }
        else
        {
            respawnPoint = platformingRespawnPoint;
        }
        
        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }
}

