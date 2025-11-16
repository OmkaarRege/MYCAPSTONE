using UnityEngine;

public class Minigame1 : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject prefabToSpawn;      // The falling prefab
    public float spawnInterval;    // Time between spawns
    private float nextSpawnTime = 0f;

    public int num = 5;

    private Camera mainCamera;
    public float moveSpeed, upspeed, initialX,initialY, screenWidth, screenHeight;

    private Vector2 screenBounds;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        mainCamera = Camera.main;
        screenHeight = mainCamera.orthographicSize * 2;
        screenWidth = screenHeight * mainCamera.aspect;
        SpriteRenderer spriteRenderer = playerObject.GetComponent<SpriteRenderer>();
        initialX = playerObject.transform.position.x;
       
    }

    void Update()
    {
        Move();
        
        Vector3 playertransform = playerObject.transform.position;
        playertransform.x = Mathf.Clamp(playertransform.x, initialX - (screenWidth/2), initialX + (screenWidth/2));
        playerObject.transform.position = playertransform;
        HandleSpawning();
    }
    
    void Move()
    {
        Vector2 movement = new Vector2(0,upspeed);

            // Check for 'A' or Left Arrow key press
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                movement.x -= 1f;
            }

            // Check for 'D' or Right Arrow key press
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                movement.x += 1f;
            }

            // Normalize the movement vector to prevent faster diagonal movement
            if (movement.magnitude > 1f)
            {
                movement.Normalize();
            }

            // Apply movement to the object's position
            playerObject.transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
    void HandleSpawning()
{
    if (Time.time >= nextSpawnTime)
    {
        SpawnFallingObject();
        nextSpawnTime = Time.time + spawnInterval;
    }
}

void SpawnFallingObject()
{
    // Random X position within screen bounds
    float randomX = Random.Range(
        initialX - (screenWidth / 2),
        initialX + (screenWidth / 2)
    );

    // Top of the camera view
    float spawnY = mainCamera.transform.position.y + (screenHeight+1);

    Vector3 spawnPosition = new Vector3(0f, spawnY, 0f);

    Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    int change = Random.value < 0.5f ? -1 : 1;
    num += change;
    num = Mathf.Clamp(num, 0, 10);  
}


    

       

        
}
