using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class Minigame1 : MonoBehaviour
{
    public GameObject playerObject,timerGO,scoreGO;
    public GameObject prefabToSpawn;      // The falling prefab
    public float spawnInterval;    // Time between spawns
    private float nextSpawnTime = 0f;

    public int num,score = 5;

    private Camera mainCamera;
    public float moveSpeed, upspeed, initialX,initialY, screenWidth, screenHeight,timer;
  
    private Vector2 screenBounds;
    private SpriteRenderer spriteRenderer;

    private float halfPlayerWidth;

    

    void Start()
    {
        mainCamera = Camera.main;
        screenHeight = mainCamera.orthographicSize * 2;
        screenWidth = screenHeight * mainCamera.aspect;
        SpriteRenderer spriteRenderer = playerObject.GetComponent<SpriteRenderer>();
        halfPlayerWidth = spriteRenderer.bounds.size.x / 2f;
        initialX = playerObject.transform.position.x;
        score=0;
        timer=15;
       
    }

    void Update()
    {
        Move();
        
        Vector3 p = playerObject.transform.position;

        float leftLimit  = mainCamera.transform.position.x - (screenWidth / 2) - 2;
        float rightLimit = mainCamera.transform.position.x + (screenWidth / 2) + 2;

        p.x = Mathf.Clamp(p.x, leftLimit, rightLimit);

        playerObject.transform.position = p;
        HandleSpawning();

        GameObject canvas = GameObject.Find("Minigame1 Canvas");
        if (canvas != null)
        {
           

           // Get the Text component
           TMP_Text scoreText = scoreGO.GetComponent<TMP_Text>();
           TMP_Text timerText = timerGO.GetComponent<TMP_Text>();

           
           scoreText.text = "Score "+score.ToString() + "/10";

           
           timerText.text = "Time left "+Mathf.CeilToInt(timer).ToString();
        }


        // Countdown timer
        timer -= Time.deltaTime;
        timer = Mathf.Max(timer, 0f);
       

        if (timer <= 0f)
        {
            SceneManager.LoadScene("MiniGame 1");
        }
        else if (timer >0f)
        {
            if (score>9)
            {
                SceneManager.LoadScene("MainScene");
            }
            
        }
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
