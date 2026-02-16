using UnityEngine;

using UnityEngine.SceneManagement;

using System.Collections;

public class Minigame3 : MonoBehaviour
{
    public GameObject flower;
    public GameObject pullString; 
    private float fixedY;
    private float fixedZ;

    private float dragOffsetX;
    private bool isDragging;

    private float startStringX;

    private Collider2D stringCol;

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

    [Header("Idle String Motion")]
    public float idleSpeed = .25f;     // cycles per second-ish (tweak)

    private float idlePhase;   
    private float idleTimer;

    private float idleMinX;
    private float idleMaxX;

    public float returnSpeed=4f;

    private bool isReturning;

    private Coroutine returnRoutine;

    [Header("Flower Rotation From String Motion")]
    public float rotatePerUnit = 500f; // degrees per unit of X movement (tweak)
    private float lastStringX;

    
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Lock Y and Z at 
        // start
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

        stringCol = pullString.GetComponent<Collider2D>();
        if (stringCol == null) stringCol = pullString.GetComponentInChildren<Collider2D>();
        float maxPullDistance = maxX - startStringX;          // how far right you can pull
        idleMinX = startStringX + (maxPullDistance *0.2f);     
        idleMaxX = startStringX + ( maxPullDistance*0.5f);

        // Safety clamp (in case of weird setup)
        idleMinX = Mathf.Clamp(idleMinX, startStringX, maxX);
        idleMaxX = Mathf.Clamp(idleMaxX, startStringX, maxX);

        SyncIdlePhaseToCurrentX();

        lastStringX = pullString.transform.position.x;
        
    }

    // Update is called once per frame
    void Update()
    {
     // ✅ Calculate offset once when mouse is pressed down
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

            if (isReturning) return;

            // Only start drag if we actually clicked the string collider (or a child collider)
            if (hit.collider != null && (hit.collider == stringCol || hit.collider.transform.IsChildOf(pullString.transform)))
            {
              // Use the hit point so offset is correct and no snapping
              dragOffsetX = pullString.transform.position.x - hit.point.x;
              isDragging = true;
            }
            Debug.Log("Clicked. Raycast hit: " + (hit.collider ? hit.collider.name : "NONE"));
            Debug.Log("Cached stringCol: " + (stringCol ? stringCol.name : "NULL"));
        }

        // ✅ Stop dragging on release
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            if (!isReturning)
            StartCoroutine(ReturnToOneFifth());
        }
        if (!isDragging&& !isReturning)
        {
          float center = (idleMinX + idleMaxX) * 0.5f;
          float amp = (idleMaxX - idleMinX) * 0.5f;

          // Smooth back-and-forth
          float x = center + amp * Mathf.Sin((Time.time * idleSpeed * Mathf.PI * 2f) + idlePhase);

          pullString.transform.position = new Vector3(x, fixedY, fixedZ);
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

            if (currentPulls >= pullsToFullColor)
            {
              SceneManager.LoadScene("MainScene");
            }

            float t = (float)currentPulls / pullsToFullColor;
            Color newColor = Color.Lerp(startColor, finalColor, t);

            SetFlowerColor(newColor);
        }

        // Re-arm once the string moves away from max, so the next full pull counts again
        if (!atMax)
        {
            pullArmed = true;
        }
        float currentX = pullString.transform.position.x;
        float dx = currentX - lastStringX;              // how much string moved this frame

        // Rotate flower in same "direction" as string motion.

        float rotationThisFrame = dx * 20;

        flower.transform.Rotate(0f, 0f, rotationThisFrame);

        lastStringX = currentX;
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
    private IEnumerator ReturnToOneFifth()
{
    isReturning = true;

    if (stringCol != null)
        stringCol.enabled = false;

    float maxPullDistance = maxX - startStringX;
    float targetX = startStringX + (maxPullDistance * 0.2f); // 1/5th

    while (Mathf.Abs(pullString.transform.position.x - targetX) > 0.1f)
    {
        float newX = Mathf.MoveTowards(
            pullString.transform.position.x,
            targetX,
            returnSpeed * Time.deltaTime
        );

        pullString.transform.position = new Vector3(newX, fixedY, fixedZ);

        yield return null;
    }

    pullString.transform.position = new Vector3(targetX, fixedY, fixedZ);

    SyncIdlePhaseToCurrentX();

    if (stringCol != null)
        stringCol.enabled = true;

    isReturning = false;
}
private void SyncIdlePhaseToCurrentX()
{
    float center = (idleMinX + idleMaxX) * 0.5f;
    float amp = (idleMaxX - idleMinX) * 0.5f;

    if (amp <= 0.0001f)
    {
        idlePhase = 0f;
        return;
    }

    float normalized = Mathf.Clamp((pullString.transform.position.x - center) / amp, -1f, 1f);
    float angle = Mathf.Asin(normalized);

    // Make sine equal current X at the current Time.time
    idlePhase = angle - (Time.time * idleSpeed * Mathf.PI * 2f);
}
            
           
        


        
        
    
    
}
