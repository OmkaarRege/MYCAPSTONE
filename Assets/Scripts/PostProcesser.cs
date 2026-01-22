using UnityEngine;
using UnityEngine.SceneManagement;

public class PostProcesser : MonoBehaviour
{
    public GameObject globalVolume;
   
    private static PostProcesser instance;

    void Awake()
    {
        // Prevent duplicates if each scene has its own copy
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Scene check
        if (globalVolume != null)
        {
            bool isMainScene = SceneManager.GetActiveScene().name == "MainScene";
            globalVolume.SetActive(isMainScene);
        }
    }
   
}
