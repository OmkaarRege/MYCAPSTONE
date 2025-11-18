using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    
    
    public void play()
    {
        SceneManager.LoadScene("MainScene");
      
        
    }

    public void quit()
    {
        Application.Quit();
    }
}
