using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    PauseMenu pauseMenu;
    public GameObject pauseMenuObject;

    void Start()
    {
        pauseMenuObject = GameObject.Find("PauseMenu");
        if (pauseMenuObject!=null)
        {
            pauseMenu=pauseMenuObject.GetComponent<PauseMenu>();
            pauseMenu.isminigame1done=false;
            pauseMenu.isminigame2done=false;
            pauseMenu.tutorialComplete=false;
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
       
    }
    
    public void play()
    {
        SceneManager.LoadScene("MainScene");
      
        
    }

    public void quit()
    {
        Application.Quit();
    }
}
