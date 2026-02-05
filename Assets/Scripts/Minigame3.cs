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

     public float currentMouseWorldX;

       [Header("Flower Rotation")]
    public float rotationMultiplier = 50f;   // strength of rotation
    public float maxRotation = 30f;           // clamp (degrees)
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Lock Y and Z at start
        fixedY = pullString.transform.position.y;
        fixedZ = pullString.transform.position.z;
        
    }

    // Update is called once per frame
    void Update()
    {
       

        // 🔹 Read mouse X when left mouse is held
        if (Input.GetMouseButton(0))
        {
        // Reusable mouse position
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.WorldToScreenPoint(pullString.transform.position).z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

         // Track mouse X
        currentMouseWorldX = worldPos.x;
        isDragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (!isDragging) return;

        Vector3 dragMousePos = Input.mousePosition;
        dragMousePos.z = Camera.main.WorldToScreenPoint(pullString.transform.position).z;
        Vector3 dragWorldPos = Camera.main.ScreenToWorldPoint(dragMousePos);

        // Apply offset, X-axis only
        float targetX = dragWorldPos.x + dragOffsetX;

        // Move string on X axis only
        pullString.transform.position = new Vector3(
            targetX,
            fixedY,
            fixedZ
        );
          // 🔹 Calculate pull direction and amount
        float pullDelta = pullString.transform.position.x - startStringX;

        // Only rotate if pulled to the RIGHT
        if (pullDelta > 0f)
        {
            float rotationAmount = Mathf.Clamp(
                pullDelta * rotationMultiplier,
                0f,
                maxRotation
            );

            flower.transform.localRotation = Quaternion.Euler(
                0f,
                0f,
                rotationAmount
            );
        }
        

            
           
        


        
        
    }
    
}
