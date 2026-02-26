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

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 2.0f; // degrees per mouse unit
    [SerializeField] private bool lockCursor = true;

    private float startYaw;
    private float yawOffset;

     private float pendingMouseX; 

    private enum Step { TurnLeft, TurnRight, Center, FreeLook }
    [SerializeField] private Step step = Step.TurnLeft;

    

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody>();

        
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

    void AdvanceStep()
    {
        if (step == Step.TurnLeft) step = Step.TurnRight;
        else if (step == Step.TurnRight) step = Step.Center;
        else if (step == Step.Center) step = Step.FreeLook; // ✅ unlock free rotation
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
