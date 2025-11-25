using UnityEngine;
using UnityEngine.SceneManagement;
public class MainPlayerMovement : MonoBehaviour
{
    public Transform cameraLocation;

    private bool canInteract = false;
    private string interactTarget = "";   // stores which interactable we're near
    public Rigidbody rb;
    public GameObject obj,pauseMenuObject;

    PauseMenu pauseMenu;

    public float movementspeed;
    Vector2 look;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        if (rb == null)
        {
            Debug.LogError("Rigidbody component not found on this GameObject.");
            enabled = false; // Disable script if no Rigidbody is found
        }

        pauseMenuObject = GameObject.Find("PauseMenu");
        if (pauseMenuObject!=null)
        {
            pauseMenu=pauseMenuObject.GetComponent<PauseMenu>();
        }


    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateLook();
        if (canInteract && Input.GetKeyDown(KeyCode.E)&&pauseMenuObject!=null)
        {
            if (interactTarget == "Interactable 1" && !pauseMenu.isminigame1done)
            {
                pauseMenu.isminigame1done = true;
                LoadMiniGame("MiniGame 1");
            }
            else if (interactTarget == "Interactable 2" && !pauseMenu.isminigame2done)
            {
                pauseMenu.isminigame2done = true;
                LoadMiniGame("MiniGame 2");
            }
        }
        
    }
    void UpdateLook()
    {
        look.x += Input.GetAxis("Mouse X");
        look.y += Input.GetAxis("Mouse Y");
        look.y = Mathf.Clamp(look.y, -89f, 89f);
        cameraLocation.localRotation = Quaternion.Euler(-look.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }
    void UpdateMovement()
    {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        var input = new Vector3();
        input += transform.forward * y;
        input += transform.right * x;
        input = Vector3.ClampMagnitude(input, 1f);
        rb.linearVelocity = input * movementspeed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Interactable"))
            return;

        // Ignore objects whose minigame is already completed
        if ((other.name == "Interactable 1" && pauseMenu.isminigame1done) ||
            (other.name == "Interactable 2" && pauseMenu.isminigame2done))
            return;

        // Store which interactable the player is near
        interactTarget = other.name;
        canInteract = true;

        // Show "Press E" text
        GameObject canvas = GameObject.Find("PlayerCanvas");
        if (canvas != null)
        {
            GameObject text = canvas.transform.Find("InteractText").gameObject;
            text.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Interactable"))
            return;

        // Reset
        canInteract = false;
        interactTarget = "";

        // Hide UI text
        GameObject canvas = GameObject.Find("PlayerCanvas");
        if (canvas != null)
        {
            GameObject text = canvas.transform.Find("InteractText").gameObject;
            text.SetActive(false);
        }
    }
    void LateUpdate()
    {
        // Get the camera's target position and the object's position
        Vector3 targetPosition = cameraLocation.transform.position;
        Vector3 objectPosition = transform.position;

        // Create a new target position with the same Y value as the object
        Vector3 lookAtTarget = new Vector3(targetPosition.x, objectPosition.y, targetPosition.z);

        if (obj!=null)
        {
        // Point the object at the new target
        obj.transform.LookAt(lookAtTarget);
        }

    }
    private void LoadMiniGame(string sceneName)
    {
        // Hide UI before loading the scene
        GameObject canvas = GameObject.Find("PlayerCanvas");
        if (canvas != null)
        {
            GameObject text = canvas.transform.Find("InteractText").gameObject;
            text.SetActive(false);
        }

        // Load the minigame scene
        SceneManager.LoadScene(sceneName);
    }
}
