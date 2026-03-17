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

    public Vector3 minigame1rotation,minigame2rotation,minigame1Pos,minigame2Pos;

    public float movementspeed;

    [SerializeField] public GameObject interactText; 

    [SerializeField] public GameObject area1Text;    // Area 1 prompt
    [SerializeField] public GameObject area2Text;    // Area 2 prompt

    private static bool mg1Loaded = false;
    private static bool mg2Loaded = false;

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

        if (pauseMenu.minigame1complete)
        {
            transform.position = new Vector3(-8,0.3f,1);
            transform.rotation = Quaternion.identity;
        }
        if (pauseMenu.minigame2complete)
        {
            transform.position = new Vector3(10,0.3f,-7.5f);
            transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        }


    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateLook();
        if (canInteract && Input.GetKeyDown(KeyCode.E)&&pauseMenuObject!=null)
        {
            if (interactTarget == "Interactable 1")
            {
                mg1Loaded=true;
                
                LoadMiniGame("MiniGame 1");
            }
            else if (interactTarget == "Interactable 2" )
            {
                mg2Loaded=true;
                LoadMiniGame("MiniGame 2");
            }
            else if (interactTarget == "Interactable 3" )
            {
                
                LoadMiniGame("MiniGame 3");
            }
        }
        
    }
    void UpdateLook()
    {
        look.x += Input.GetAxis("Mouse X")*2;
        look.y += Input.GetAxis("Mouse Y")*2;
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
    // 1) Area prompts (UI only)
    if (other.CompareTag("InteractableTag1"))
    {
        if (area1Text != null&&!mg1Loaded) area1Text.SetActive(true);
        return;
    }

    if (other.CompareTag("InteractableTag2"))
    {
        if (area2Text != null&&!mg2Loaded) area2Text.SetActive(true);
        return;
    }

    // 2) Minigame interactables
    if (!other.CompareTag("Interactable"))
        return;

    interactTarget = other.name;
    canInteract = true;

    // Show "Press E"
    if (interactText != null)
        interactText.SetActive(true);
}

    private void OnTriggerExit(Collider other)
{
    // Area prompts
    if (other.CompareTag("InteractableTag1"))
    {
        if (area1Text != null) area1Text.SetActive(false);
        return;
    }

    if (other.CompareTag("InteractableTag2"))
    {
        if (area2Text != null) area2Text.SetActive(false);
        return;
    }

    // Minigame interactables
    if (!other.CompareTag("Interactable"))
        return;

    canInteract = false;
    interactTarget = "";

    if (interactText != null)
        interactText.SetActive(false);
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
