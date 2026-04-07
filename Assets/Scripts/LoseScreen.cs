using UnityEngine;
using UnityEngine.SceneManagement;
public class LoseScreen : MonoBehaviour
{
    public string retrySceneName;
    
    void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
    }

    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void PlayAgain()
    {
    Time.timeScale = 1f;
    SceneManager.LoadScene(retrySceneName);
    }

    public void ReturnToMainMenu()
    {
    Time.timeScale = 1f;
    SceneManager.LoadScene("MainMenu");
    }
}
