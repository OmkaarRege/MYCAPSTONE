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

    [SerializeField] public GameObject rightKeyImg;
    [SerializeField] private RectTransform rightKeyRect;

     [Header("Key Pulse")]
    [SerializeField] private float keyMinSize = 100f;
    [SerializeField] private float keyMaxSize = 120f;
    [SerializeField] private float keyPulseDuration = 0.6f;

    private float downKeyTimer;

     [Header("Move Left Requirement")]
    [SerializeField] public float moveLeftDistance = 0.75f;     // how far to move left
    [SerializeField] public float moveSpeed = 5f;            // move speed during tutorial

    [SerializeField] private float moveAccel = 20f; // how quickly it reaches target speed
    private float moveVelX; // current left speed (for smoothing)

    private Vector3 tutorialRight;
    private Vector3 tutorialForward;

    [Header("Back Move UI")]
    [SerializeField] public GameObject downKeyImg;
    [SerializeField] private RectTransform downKeyRect;

    [SerializeField] public GameObject downArrowImg;
    [SerializeField] private RectTransform downArrowRect;

    [Header("Forward Move UI")]
    [SerializeField] public GameObject upKeyImg;
    [SerializeField] private RectTransform upKeyRect;

    [SerializeField] public GameObject upArrowImg;
    [SerializeField] private RectTransform upArrowRect;

    [SerializeField] public MainPlayerMovement mainPlayerMovement;

    private bool clampHorizontalMovement = false;
    private float pendingMoveX;

    private float arrowTimer;
    private float keyTimer; 

    private float startYaw;
    private float yawOffset;

     private float pendingMouseX; 

     private Vector3 moveStartPos;

     private Vector3 tutorialCenterPos;
    private float rightKeyTimer;
    
    private float upKeyTimer;

    private enum Step { TurnLeft, TurnRight, Center,MoveLeft,MoveRight,MoveBack, MoveForward, FreeLook }
    [SerializeField] private Step step = Step.TurnLeft;

    

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();

        if (!arrowRect && arrowImg) arrowRect = arrowImg.GetComponent<RectTransform>();    
        if (!arrowRect_1 && arrowImg_1) arrowRect_1 = arrowImg_1.GetComponent<RectTransform>();        
        if (!leftKeyRect && leftKeyImg) leftKeyRect = leftKeyImg.GetComponent<RectTransform>();
        if (!rightKeyRect && rightKeyImg) rightKeyRect = rightKeyImg.GetComponent<RectTransform>();
        if (!downKeyRect && downKeyImg) downKeyRect = downKeyImg.GetComponent<RectTransform>();
        if (!downArrowRect && downArrowImg) downArrowRect = downArrowImg.GetComponent<RectTransform>();
        if (!upKeyRect && upKeyImg) upKeyRect = upKeyImg.GetComponent<RectTransform>();
        if (!upKeyRect && upKeyImg) upKeyRect = upKeyImg.GetComponent<RectTransform>();
    }

    void Start()
    {
        if (PauseMenu.instance != null && PauseMenu.instance.tutorialComplete)
        {
            mainPlayerMovement.enabled = true;
            clampHorizontalMovement = false;
            enabled = false;
        }
        else
        {
            mouseImg.SetActive(true);
            arrowImg.SetActive(true);
        }

        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Smooth render between fixed physics steps (helps with snapping/jitter)
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        startYaw = rb.rotation.eulerAngles.y;
        yawOffset = 0f;
        tutorialCenterPos = rb.position;
        // Apply initial rotation through physics-safe call
        rb.MoveRotation(Quaternion.Euler(0f, startYaw, 0f));
        tutorialRight = transform.right;
        tutorialForward = transform.forward;
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
        else if (step == Step.MoveRight && arrowRect_1 != null && arrowImg_1 != null && arrowImg_1.activeSelf)
        {
          AnimateArrow(arrowRect_1, 120f, 250f);
        }
        if (step == Step.MoveRight && rightKeyRect != null && rightKeyImg != null && rightKeyImg.activeSelf)
        {
           rightKeyTimer += Time.unscaledDeltaTime;
           float t = Mathf.PingPong(rightKeyTimer / Mathf.Max(0.0001f, keyPulseDuration), 1f);
           t = Mathf.SmoothStep(0f, 1f, t);
           float s = Mathf.Lerp(keyMinSize, keyMaxSize, t);
           rightKeyRect.sizeDelta = new Vector2(s, s);
       }
       else if (step == Step.MoveBack && downArrowRect != null && downArrowImg != null && downArrowImg.activeSelf)
       {
           AnimateArrowY(downArrowRect, -140f, -250f);
       }
       if (step == Step.MoveBack && downKeyRect != null && downKeyImg != null && downKeyImg.activeSelf)
       {
          downKeyTimer += Time.unscaledDeltaTime;
          float t = Mathf.PingPong(downKeyTimer / Mathf.Max(0.0001f, keyPulseDuration), 1f);
          t = Mathf.SmoothStep(0f, 1f, t);
          float s = Mathf.Lerp(keyMinSize, keyMaxSize, t);
          downKeyRect.sizeDelta = new Vector2(s, s);
      }
      else if (step == Step.MoveForward && upArrowRect != null && upArrowImg != null && upArrowImg.activeSelf)
      {
          AnimateArrowY(upArrowRect, 140f, 250f);
      }
      if (step == Step.MoveForward && upKeyRect != null && upKeyImg != null && upKeyImg.activeSelf)
      {
         upKeyTimer += Time.unscaledDeltaTime;
         float t = Mathf.PingPong(upKeyTimer / Mathf.Max(0.0001f, keyPulseDuration), 1f);
         t = Mathf.SmoothStep(0f, 1f, t);
         float s = Mathf.Lerp(keyMinSize, keyMaxSize, t);
         upKeyRect.sizeDelta = new Vector2(s, s);
     }
    }
    void FixedUpdate()
    {
        // Consume input once per physics step
        float deltaYaw = pendingMouseX;
        pendingMouseX = 0f;

        if (step == Step.FreeLook)
        {
           mainPlayerMovement.enabled = true;
           enabled=false;        
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

                step = Step.MoveRight;

                if (rightKeyImg) rightKeyImg.SetActive(true);
                if (arrowImg_1) arrowImg_1.SetActive(true);

                arrowTimer = 0f;
                rightKeyTimer = 0f;

                var v = rb.linearVelocity;
                rb.linearVelocity = new Vector3(0f, v.y, 0f);
            }

            
        }
        else if (step == Step.MoveRight)
        {
            ApplyYawPhysics();

            float h = Input.GetAxis("Horizontal");
            float rightInput = Mathf.Clamp01(h); // D / right = positive

            Vector3 vel = transform.right * (rightInput * moveSpeed);
            vel.y = rb.linearVelocity.y;
            rb.linearVelocity = vel;

            float movedRight = Vector3.Dot(rb.position - tutorialCenterPos, transform.right);
            if (movedRight >= moveLeftDistance)
            {
              if (rightKeyImg) rightKeyImg.SetActive(false);
              if (arrowImg_1) arrowImg_1.SetActive(false);

              step = Step.MoveBack;
              clampHorizontalMovement = true;
              if (downKeyImg) downKeyImg.SetActive(true);
              if (downArrowImg) downArrowImg.SetActive(true);
              arrowTimer = 0f;
              downKeyTimer = 0f;

              
              var v = rb.linearVelocity;
              rb.linearVelocity = new Vector3(0f, v.y, 0f);
            } 

           
        }
        else if (step == Step.MoveBack)
        {
          ApplyYawPhysics();

          float h = Input.GetAxis("Horizontal");
          float vInput = Input.GetAxis("Vertical");

          float backInput = Mathf.Clamp01(-vInput); // S / Down = -1 -> 1

           Vector3 input = Vector3.zero;
           input += tutorialRight * h;
           input += -tutorialForward * backInput;
           input = Vector3.ClampMagnitude(input, 1f);

           Vector3 vel = input * moveSpeed;
           vel.y = rb.linearVelocity.y;
           rb.linearVelocity = vel;

           float movedBack = Vector3.Dot(rb.position - tutorialCenterPos, -tutorialForward);
           if (movedBack >= moveLeftDistance)
           {
             if (downKeyImg) downKeyImg.SetActive(false);
             if (downArrowImg) downArrowImg.SetActive(false);

            step = Step.MoveForward;

            if (upKeyImg) upKeyImg.SetActive(true);
            if (upArrowImg) upArrowImg.SetActive(true);

            arrowTimer = 0f;
            upKeyTimer = 0f;

             var finalVel = rb.linearVelocity;
             rb.linearVelocity = new Vector3(0f, finalVel.y, 0f);
            }
        }
        else if (step == Step.MoveForward)
        {
          ApplyYawPhysics();

          float h = Input.GetAxis("Horizontal");
          float vInput = Input.GetAxis("Vertical");

          float forwardInput = Mathf.Clamp01(vInput); // W / Up = positive

          Vector3 input = Vector3.zero;
          input += tutorialRight * h;
          input += tutorialForward * forwardInput;
          input = Vector3.ClampMagnitude(input, 1f);

          Vector3 vel = input * moveSpeed;
          vel.y = rb.linearVelocity.y;
          rb.linearVelocity = vel;

          float currentBackOffset = Vector3.Dot(rb.position - tutorialCenterPos, -tutorialForward);

          // complete when player comes forward to center line
          if (currentBackOffset <= 0.05f)
          {
            if (upKeyImg) upKeyImg.SetActive(false);
            if (upArrowImg) upArrowImg.SetActive(false);

            step = Step.FreeLook;
            clampHorizontalMovement = false;
            if (PauseMenu.instance != null)
            {
               PauseMenu.instance.tutorialComplete = true;
            }


            var finalVel = rb.linearVelocity;
            rb.linearVelocity = new Vector3(0f, finalVel.y, 0f);
         }
        }
        
        else{
        // Tutorial-gated rotation
        yawOffset = ClampForStep(yawOffset + deltaYaw);
        ApplyYawPhysics();

        if (IsStepComplete())
            AdvanceStep();
        }
        if (clampHorizontalMovement)
        {
          Vector3 offset = rb.position - tutorialCenterPos;

          float horizontalOffset = Vector3.Dot(offset, tutorialRight);
          float clampedHorizontal = Mathf.Clamp(horizontalOffset, -moveLeftDistance, moveLeftDistance);

          float backOffset = Vector3.Dot(offset, -tutorialForward);
          float clampedBack = Mathf.Clamp(backOffset, 0f, moveLeftDistance);

          float verticalAmount = rb.position.y - tutorialCenterPos.y;

          // Stop pushing into left/right extremes
          float horizontalVelocity = Vector3.Dot(rb.linearVelocity, tutorialRight);

          if (horizontalOffset <= -moveLeftDistance && horizontalVelocity < 0f)
          {
             Vector3 v = rb.linearVelocity;
             v -= tutorialRight * horizontalVelocity;
             rb.linearVelocity = v;
          }
         else if (horizontalOffset >= moveLeftDistance && horizontalVelocity > 0f)
          {
            Vector3 v = rb.linearVelocity;
            v -= tutorialRight * horizontalVelocity;
            rb.linearVelocity = v;
          }

          // Stop pushing into back extreme
         float backVelocity = Vector3.Dot(rb.linearVelocity, -tutorialForward);

         if (backOffset >= moveLeftDistance && backVelocity > 0f)
        {
            Vector3 v = rb.linearVelocity;
            v -= (-tutorialForward) * backVelocity;
            rb.linearVelocity = v;
        }

        Vector3 clampedPosition =
        tutorialCenterPos
        + tutorialRight * clampedHorizontal
        - tutorialForward * clampedBack
        + Vector3.up * verticalAmount;

       rb.MovePosition(clampedPosition);
}
        
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
    void AnimateArrowY(RectTransform rect, float y1, float y2)
    {
    arrowTimer += Time.unscaledDeltaTime;

    float t = Mathf.PingPong(arrowTimer / Mathf.Max(0.0001f, arrowDuration), 1f);
    t = Mathf.SmoothStep(0f, 1f, t);

    float y = Mathf.Lerp(y1, y2, t);
    Vector2 pos = rect.anchoredPosition;
    pos.y = y;
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
            Step.MoveLeft => "Press A to move LEFT.",
            Step.MoveRight => "Now press D to move RIGHT.",
            Step.MoveBack => "Press S to move BACK.",
            Step.FreeLook => "", // hide text after tutorial
            _ => ""
        };
    }

    public bool TutorialComplete => step == Step.FreeLook;
}
