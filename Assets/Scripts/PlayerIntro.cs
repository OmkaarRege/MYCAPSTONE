using System.Collections;
using UnityEngine;

public class PlayerIntro : MonoBehaviour
{
 [Header("References")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform yawRoot; // usually the Player transform

    [Header("Tutorial Targets (degrees yaw relative to start)")]
    [SerializeField] private float leftTarget = -45f;
    [SerializeField] private float rightTarget = 45f;
    [SerializeField] private float angleTolerance = 3f;

    [Header("Tutorial UI (shown only during LEFT step)")]
    [SerializeField] public GameObject mouseImg;          // mouse icon on canvas
    [SerializeField] public GameObject arrowImg;          // arrow icon on canvas
    [SerializeField] private RectTransform arrowRect;      // RectTransform of arrowImg

     [SerializeField] public GameObject arrowImg_1;          // arrow icon on canvas
    [SerializeField] private RectTransform arrowRect_1;      // RectTransform of arrowImg

    [Header("Arrow Motion (anchored X ping-pong)")]
    
    [SerializeField] private float arrowDuration = 1.2f;   // seconds one way

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 2.0f; // degrees per mouse unit
    [SerializeField] private bool lockCursor = true;

    [Header("Left Move UI")]
    [SerializeField] public GameObject leftKeyImg;
    [SerializeField] private RectTransform leftKeyRect;

     [Header("Key Pulse")]
    [SerializeField] private float keyMinSize = 100f;
    [SerializeField] private float keyMaxSize = 120f;
    [SerializeField] private float keyPulseDuration = 0.6f;

     [Header("Move Left Requirement")]
    [SerializeField] public float moveLeftDistance = 1f;     // how far to move left
    [SerializeField] public float moveSpeed = 100f;            // move speed during tutorial

    [SerializeField] private float moveAccel = 20f; // how quickly it reaches target speed
    private float moveVelX; // current left speed (for smoothing)


    private float pendingMoveX;

    private float arrowTimer;
    private float keyTimer; 

    private float startYaw;
    private float yawOffset;

     private float pendingMouseX; 

     private Vector3 moveStartPos;

    private enum Step { TurnLeft, TurnRight, Center,MoveLeft, FreeLook }
    [SerializeField] private Step step = Step.TurnLeft;

    

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();

        if (!arrowRect && arrowImg) arrowRect = arrowImg.GetComponent<RectTransform>();    
        if (!arrowRect_1 && arrowImg_1) arrowRect_1 = arrowImg_1.GetComponent<RectTransform>();        
        if (!leftKeyRect && leftKeyImg) leftKeyRect = leftKeyImg.GetComponent<RectTransform>();
    
    }

    void Start()
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Smooth render between fixed physics steps (helps with snapping/jitter)
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        startYaw = rb.rotation.eulerAngles.y;
        yawOffset = 0f;

        // Apply initial rotation through physics-safe call
        rb.MoveRotation(Quaternion.Euler(0f, startYaw, 0f));
    }
    

    void Update()
    {
        // Accumulate mouse input in Update (render rate)
        float mouseX = Input.GetAxisRaw("Mouse X");
        pendingMouseX += mouseX * mouseSensitivity;

        pendingMoveX = Input.GetAxis("Horizontal");

        if (step == Step.TurnLeft && arrowRect != null && arrowImg != null && arrowImg.activeSelf)
        {
            AnimateArrow(arrowRect, -120f, -250f);
        }
        else if (step == Step.TurnRight && arrowRect_1 != null && arrowImg_1!=null&& arrowImg_1.activeSelf)
        {
            AnimateArrow(arrowRect_1, 120f, 250f);
        }
        else if (step == Step.MoveLeft && arrowRect != null && arrowImg != null && arrowImg.activeSelf)
        {
            AnimateArrow(arrowRect, -120f, -250f);
        }
        if (step == Step.MoveLeft && leftKeyRect != null && leftKeyImg != null && leftKeyImg.activeSelf)
        {
            keyTimer += Time.unscaledDeltaTime;
            float t = Mathf.PingPong(keyTimer / Mathf.Max(0.0001f, keyPulseDuration), 1f);
            t = Mathf.SmoothStep(0f, 1f, t);
            float s = Mathf.Lerp(keyMinSize, keyMaxSize, t);
            leftKeyRect.sizeDelta = new Vector2(s, s);
        }
    }
    void FixedUpdate()
    {
        // Consume input once per physics step
        float deltaYaw = pendingMouseX;
        pendingMouseX = 0f;

        if (step == Step.FreeLook)
        {
            // ✅ Free rotation
            yawOffset += deltaYaw;
            ApplyYawPhysics();
            return;
        }
        if (step == Step.MoveLeft)
        {
            ApplyYawPhysics();

            // Move left while holding A / left input
            float leftInput = Mathf.Clamp01(-pendingMoveX); // A gives -1 -> 1

            // Smoothly ramp speed toward target
            float targetSpeed = leftInput * moveSpeed;
            moveVelX = Mathf.MoveTowards(moveVelX, targetSpeed, moveAccel * Time.fixedDeltaTime);

            // Apply smoothed velocity
            Vector3 vel = (-transform.right) * moveVelX;
            vel.y = rb.linearVelocity.y;
            rb.linearVelocity = vel;



            float movedLeft = Vector3.Dot(rb.position - moveStartPos, -transform.right);
            if (movedLeft >= moveLeftDistance)
            {
                // Done: hide key + arrow, unlock free look
                if (leftKeyImg) leftKeyImg.SetActive(false);
                if (arrowImg) arrowImg.SetActive(false);

                step = Step.FreeLook;

                // stop horizontal velocity
                var v = rb.linearVelocity;
                rb.linearVelocity = new Vector3(0f, v.y, 0f);
            }

            return;
        }

        // Tutorial-gated rotation
        yawOffset = ClampForStep(yawOffset + deltaYaw);
        ApplyYawPhysics();

        if (IsStepComplete())
            AdvanceStep();
        
    }

    float ClampForStep(float offset)
    {
        switch (step)
        {
            case Step.TurnLeft:
                // Only allow 0 -> leftTarget (negative)
                return Mathf.Clamp(offset, leftTarget, 0f);

            case Step.TurnRight:
                // Only allow leftTarget -> rightTarget
                return Mathf.Clamp(offset, leftTarget, rightTarget);

            case Step.Center:
                // Only allow rightTarget -> 0 (back to center)
                return Mathf.Clamp(offset, 0f, rightTarget);

            default:
                return offset;
        }
    }

    bool IsStepComplete()
    {
        float target = step switch
        {
            Step.TurnLeft => leftTarget,
            Step.TurnRight => rightTarget,
            Step.Center => 0f,
            _ => yawOffset
        };

        return Mathf.Abs(yawOffset - target) <= angleTolerance;
    }

     void AnimateArrow(RectTransform rect, float x1, float x2)
    {
        arrowTimer += Time.unscaledDeltaTime;

        float t = Mathf.PingPong(arrowTimer / Mathf.Max(0.0001f, arrowDuration), 1f);
        t = Mathf.SmoothStep(0f, 1f, t);

        float x = Mathf.Lerp(x1, x2, t);
        Vector2 pos = rect.anchoredPosition;
        pos.x = x;
        rect.anchoredPosition = pos;
    }


    void AdvanceStep()
    {
        if (step == Step.TurnLeft)
        {
            step = Step.TurnRight;
                    // mouse icon on canvas
            arrowImg.SetActive(false);
            arrowImg_1.SetActive(true);

        }
        else if (step == Step.TurnRight)
        {
            step = Step.Center;
            mouseImg.SetActive(false); 
            arrowImg_1.SetActive(false);

        } 
        
        else if (step == Step.Center)
        {
            step = Step.MoveLeft;

            moveStartPos = rb.position;

            if (leftKeyImg) leftKeyImg.SetActive(true);
            if (arrowImg) arrowImg.SetActive(true);   // left arrow again
            moveVelX = 0f;
            arrowTimer = 0f;
            keyTimer = 0f;
        } 
        
    }

    void ApplyYawPhysics()
    {
        float yaw = startYaw + yawOffset;
        rb.rotation = Quaternion.Euler(0f, yaw, 0f);
    }

    // Optional: show tutorial instructions in UI
    public string GetInstructionText()
    {
        return step switch
        {
            Step.TurnLeft => "Move the mouse LEFT to turn.",
            Step.TurnRight => "Now move the mouse RIGHT to turn.",
            Step.Center => "Return to CENTER.",
            Step.FreeLook => "", // hide text after tutorial
            _ => ""
        };
    }

    public bool TutorialComplete => step == Step.FreeLook;
}
