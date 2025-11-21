using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerAnimationController))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    [SerializeField] private float maxStamina = 10f;
    [SerializeField] private float staminaSubAmount = 0.1f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpForwardMultiplier = 0.6f;

    [Header("Jump Timing")]
    [SerializeField] private bool useAnimationEvent = true;
    [SerializeField] private float jumpDelay = 0.12f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private UnityEngine.UI.Image staminaBar;

    private Rigidbody rb;
    private PlayerAnimationController animController;
    private Animator animator;
    private Coroutine jumpCoroutine;

    private Vector2 moveInput;
    private Vector2 smoothedInput;
    public Vector3 moveDirection;
    private Vector3 velocitySmoothRef;
    private bool isRunning;
    private float currentSpeed;
    private bool wasGroundedLastFrame = true;
    private float currentStamina;
    private float staminaThreshold = 0.5f;
    private Action input;

    private bool canStaminaRecover=false;
    private float staminaRecoverEffectTime=10f;
    private float currentStaminaTimeCounter=0f;
    private float staminaRecover=0.1f;
    private bool isMovementLocked = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.freezeRotation = true;

        animController = GetComponent<PlayerAnimationController>();
        animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();
        animator.applyRootMotion = false; //  KHÔNG dùng root motion

        currentStamina = maxStamina;
        input = new Action();
    }

    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    private void Update()
    {
        Vector2 rawInput = input.Player.Move.ReadValue<Vector2>();
        isRunning = input.Player.Run.IsPressed();

        if (isRunning)
            currentStamina = Mathf.Clamp(currentStamina - staminaSubAmount * Time.deltaTime, 0, maxStamina);

        smoothedInput = Vector2.Lerp(smoothedInput, rawInput, Time.deltaTime * 10f);
        moveInput = smoothedInput;

        // --- Hướng theo camera ---
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        moveDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        // --- Tốc độ ---
        currentSpeed = (isRunning && currentStamina > staminaThreshold) ? runSpeed : walkSpeed;
        if (currentStamina < staminaThreshold&&isRunning==true)
        {
            isRunning = false;
        }
        animController.UpdateMovement(moveInput, isRunning);

        // --- Nhảy ---
        if (input.Player.Jump.triggered && IsGrounded())
        {
            if (animController.CanPlayJump())
            {
                animController.PlayJump();

                if (!useAnimationEvent)
                {
                    if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
                    jumpCoroutine = StartCoroutine(DelayedJumpRoutine(jumpDelay));
                }
            }
        }
        if (canStaminaRecover)
        {
            if (currentStamina < maxStamina)
            {
                if (currentStaminaTimeCounter < staminaRecoverEffectTime)
                {
                    currentStamina = Mathf.Clamp(currentStamina +staminaRecover * Time.deltaTime, 0, maxStamina);
                    currentStaminaTimeCounter += Time.deltaTime;
                }
                else
                {
                    canStaminaRecover = false;
                }
            }
        }
        // setting stammina bar
        staminaBar.fillAmount =Mathf.Clamp((float) currentStamina / maxStamina,0,1);
    }

    private void FixedUpdate()
    {
        bool grounded = IsGrounded();

        Vector3 currentVel = rb.linearVelocity;
        Vector3 targetDir = moveDirection;
        if (!grounded && rb.linearVelocity.y < 0)
        {
            Debug.Log("Falling");
            // Khi đang rơi, tăng tốc rơi
            rb.AddForce(Vector3.down * 100f, ForceMode.Acceleration);
        }
        if ( input.Player.Move.ReadValue<Vector2>().sqrMagnitude>0.01f)
        {
            // Nếu trước mặt không bị chặn
            if (!IsFrontBlocked())
            {
                float targetSpeed = currentSpeed;
                // Tính vận tốc hiện tại theo hướng di chuyển
                Vector3 horizontalVel = new Vector3(currentVel.x, currentVel.y, currentVel.z);
                float currentSpeedInDir = Vector3.Dot(horizontalVel, targetDir);

                // Tính lực cần thêm để đạt targetSpeed
                float speedDiff = targetSpeed - currentSpeedInDir;
                Vector3 force = targetDir * (speedDiff * acceleration);

          

                // Add lực theo hướng di chuyển
                rb.AddForce(force, ForceMode.Acceleration);
            }
            else
            {
                // Nếu trước mặt bị chặn => dừng ngang
                rb.linearVelocity = new Vector3(0, currentVel.y, 0);
            }
        }
        else
        {
            // Nếu không có input, giảm tốc dần (mượt)
            Vector3 slowed = new Vector3(currentVel.x, 0, currentVel.z);
            //slowed = Vector3.Lerp(slowed, Vector3.zero, Time.fixedDeltaTime * deceleration);
            slowed = Vector3.zero;
            rb.linearVelocity = new Vector3(slowed.x, currentVel.y, slowed.z);
        }

        // Cập nhật animation nhảy khi chạm đất
        if (grounded && !wasGroundedLastFrame)
            animController.EndJump();

        wasGroundedLastFrame = grounded;

    }

    private bool IsFrontBlocked()
    {
        float checkDistance = (isRunning ? 0.8f : 0.5f);
        // Bỏ qua collider trigger
        return Physics.Raycast(
            transform.position + Vector3.up * 0.5f,
            moveDirection,
            checkDistance,
            ~0,
            QueryTriggerInteraction.Ignore
        );
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.3f);
    }

    private IEnumerator DelayedJumpRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (IsGrounded()) Jump();
        jumpCoroutine = null;
    }

    public void OnJumpAnimationEvent()
    {
        if (IsGrounded()) Jump();
    }

    private void Jump()
    {
        Vector3 horizontalVel = rb.linearVelocity;
        horizontalVel.y = 0f;

        Vector3 jumpDir = horizontalVel.sqrMagnitude > 0.1f ? horizontalVel.normalized : moveDirection;
        Vector3 jumpForceVector = Vector3.up * jumpForce;

        if (jumpDir.sqrMagnitude > 0.01f)
            jumpForceVector += jumpDir * (jumpForce * jumpForwardMultiplier);

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); // reset Y
        rb.AddForce(jumpForceVector, ForceMode.VelocityChange);
    }

    public void AddStamina(float value)
    {
        currentStamina = Mathf.Clamp(currentStamina + value, 0, maxStamina);
    }
    public void AddSpeed(float value)
    {
        runSpeed += value;
        walkSpeed += value;
    }
    public void AddStaminaPerSecond(float value)
    {
        staminaRecover = value;
        canStaminaRecover = true;
    }
    public void LockMovement()
    {
        input.Player.Disable();
        isMovementLocked = true; 
        rb.linearVelocity = Vector3.zero;
        animController.UpdateMovement(Vector2.zero, false);
    }

    public void UnlockMovement()
    {
        input.Player.Enable();
        isMovementLocked = false;
    }
}
