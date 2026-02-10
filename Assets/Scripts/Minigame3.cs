using UnityEngine;

public class Minigame3 : MonoBehaviour
{
    public GameObject flower;
    public GameObject pullString;

    
    private float fixedY;
    private float fixedZ;

    private float dragOffsetX;
    private bool isDragging;

    private float startStringX;

    public float maxX = 0.25f;
     public float maxDetectEpsilon = 0.001f;   

     public float currentMouseWorldX;

       [Header("Flower Rotation")]
    public float rotationMultiplier = 50f;   // strength of rotation
    public float maxRotation = 30f;           // clamp (degrees)
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Flower Color Lighten Per Max Pull")]
    public Color finalColor = new Color32(0xDD, 0x00, 0xFF, 0xFF); // #DD00FF
    [Range(0f, 1f)] public float startDarkFactor = 0.3f;          // 0.3 = 30% brightness
    public int pullsToFullColor = 3;

    private int currentPulls = 0;
    private bool pullArmed = true; // prevents counting multiple times while held at max
    private Color startColor;

    // Supports either 2D or 3D flower rendering
    private SpriteRenderer flowerSprite;
    private Renderer flowerMeshRenderer;

    
    void Start()
    {
        // Lock Y and Z at start
        fixedY = pullString.transform.position.y;
        fixedZ = pullString.transform.position.z;

        startStringX = pullString.transform.position.x;

        // Cache renderers
        flowerSprite = flower.GetComponent<SpriteRenderer>();
        flowerMeshRenderer = flower.GetComponent<Renderer>();

        // Compute darker start color
        startColor = new Color(
            finalColor.r * startDarkFactor,
            finalColor.g * startDarkFactor,
            finalColor.b * startDarkFactor,
            1f
        );

        SetFlowerColor(startColor);
        
    }

    // Update is called once per frame
    void Update()
    {
     // ✅ Calculate offset once when mouse is pressed down
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(pullString.transform.position).z;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            dragOffsetX = pullString.transform.position.x - worldPos.x;
            isDragging = true;
        }

        // ✅ Stop dragging on release
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        // Track mouse X while held (optional)
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.WorldToScreenPoint(pullString.transform.position).z;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            currentMouseWorldX = worldPos.x;
        }

        if (!isDragging) return;

        // ✅ Move while held, using the stored offset
        Vector3 dragMousePos = Input.mousePosition;
        dragMousePos.z = Camera.main.WorldToScreenPoint(pullString.transform.position).z;

        Vector3 dragWorldPos = Camera.main.ScreenToWorldPoint(dragMousePos);

        float targetX = dragWorldPos.x + dragOffsetX;
        targetX = Mathf.Min(targetX, maxX);  

        pullString.transform.position = new Vector3(targetX, fixedY, fixedZ);

        // ✅ Rotate flower only when pulled right
        float pullDelta = pullString.transform.position.x - startStringX;

        if (pullDelta > 0f)
        {
             float rotationSpeed = pullDelta * rotationMultiplier;

    flower.transform.Rotate(
        0f,
        0f,
        rotationSpeed * Time.deltaTime
    );
        }
        bool atMax = pullString.transform.position.x >= (maxX - maxDetectEpsilon);

        if (atMax && pullArmed)
        {
            pullArmed = false;

            currentPulls = Mathf.Clamp(currentPulls + 1, 0, pullsToFullColor);

            float t = (float)currentPulls / pullsToFullColor;
            Color newColor = Color.Lerp(startColor, finalColor, t);

            SetFlowerColor(newColor);
        }

        // Re-arm once the string moves away from max, so the next full pull counts again
        if (!atMax)
        {
            pullArmed = true;
        }
    }
    private void SetFlowerColor(Color c)
    {
        if (flowerSprite != null)
        {
            flowerSprite.color = c;
            return;
        }

        if (flowerMeshRenderer != null && flowerMeshRenderer.material != null)
        {
            // Most shaders use material.color / _Color
            flowerMeshRenderer.material.color = c;
        }
    }

            
           
        


        
        
    
    
}
