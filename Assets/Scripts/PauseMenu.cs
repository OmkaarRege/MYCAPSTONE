using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    public bool ispaused;

    private static PauseMenu instance; // singleton to avoid duplicates

    void Awake()
    {
        // If an instance already exists, destroy duplicate
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        // Make this object persist across scene loads
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
            if (ispaused)
            {
                resumegame();
            }
            else
            {
                pausegame();
            }
        }
        }
    }

    public void pausegame()
    {
        pauseMenu.SetActive(true);
         Cursor.lockState = CursorLockMode.None; // Unlock cursor
         Cursor.visible = true;   
        Time.timeScale = 0f;
        ispaused=true;
    }
    public void mainmenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
        
    }
    public void resumegame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked; // Lock if your game uses mouse control
        Cursor.visible = false;
        ispaused=false;
    }
}
