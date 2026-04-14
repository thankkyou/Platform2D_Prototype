
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public string transitionedFromScene;

    public string nextSpawnID;

    // public int savedHealth;
    // public int savedPotions;

    public Vector2 platformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] CheckPoint checkPoint;

    [SerializeField] private FadeUI pauseMenu;
    [SerializeField] private float fadeTime;
    public bool gameIsPaused;

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
        Application.targetFrameRate = 60;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Tự động tìm và đăng ký PauseMenu sau mỗi scene load
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        pauseMenu = null;
        PauseMenuRegistrar registrar = FindFirstObjectByType<PauseMenuRegistrar>(FindObjectsInactive.Include);
        if (registrar != null)
        {
            FadeUI fadeUI = registrar.GetComponent<FadeUI>();
            if (fadeUI != null)
            {
                SetPauseMenu(fadeUI);
                Debug.Log("[GameManager] Đã tìm thấy và gán PauseMenu tự động.");
            }
        }
        else
        {
            Debug.LogWarning("[GameManager] Không tìm thấy PauseMenuRegistrar trong scene: " + scene.name);
        }
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

        // Death Screen tự ẩn khi nút Respawn được nhấn (xử lý trong DeathScreenController.OnRespawnClicked)

        PlayerController.Instance.Respawned();
    }

    public void OnRespawnClicked()
    {
        // Ẩn GameOverOverlay
        DeathScreenController dsc = DeathScreenController.FindCurrent();
        if (dsc != null)
            dsc.gameObject.SetActive(false);
        else
        {
            // Fallback: tìm qua UIManager nếu không có DeathScreenController
            if (UIManager.Instance != null)
                StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        }

        RespawnPlayer();
    }

    public void Update()
    {
       if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu == null)
            {
                Debug.LogWarning("[GameManager] pauseMenu là null! Kiểm tra PauseMenuRegistrar đã được gắn vào Pause Menu trong scene chưa.");
                return;
            }

            if (!gameIsPaused)
            {
                pauseMenu.FadeUIIn(fadeTime);
                Time.timeScale = 0;
                gameIsPaused = true;
            }
            else
            {
                pauseMenu.FadeUIOut(fadeTime);
                UnPausedGame();
            }
        }
    }

    public void SetPauseMenu(FadeUI menu)
    {
        pauseMenu = menu;
    }

    public void UnPausedGame()
    {
        if (pauseMenu != null)
            pauseMenu.FadeUIOut(fadeTime);
        Time.timeScale = 1;
        gameIsPaused = false;
    }
}

