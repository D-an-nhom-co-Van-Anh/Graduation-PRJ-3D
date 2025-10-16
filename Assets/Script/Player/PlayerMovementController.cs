using System.Collections;
using UnityEngine;

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
    [Tooltip("Nếu true => dùng Animation Event (recommended). Nếu false => dùng delay cố định.")]
    [SerializeField] private bool useAnimationEvent = true;
    [Tooltip("Chỉ dùng khi useAnimationEvent = false")]
    [SerializeField] private float jumpDelay = 0.12f;

    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private PlayerAnimationController animController;
    private Animator animator;
    private Coroutine jumpCoroutine;

    private Vector2 moveInput;
    private Vector2 smoothedInput;
    public Vector3 moveDirection;
    private Vector3 currentVelocity;
    private bool isRunning;
    private float currentSpeed;
    private bool wasGroundedLastFrame = true;
    private float currentStamina;
    private float staminaThreshold=0.5f;
    
    // --- Input wrapper (giữ nguyên cách bạn dùng input/action) ---
    private Action input;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animController = GetComponent<PlayerAnimationController>();
        animator = GetComponent<Animator>() ?? GetComponentInChildren<Animator>();

        // Khóa rotation vật lý để tránh Rigidbody tự xoay
        rb.freezeRotation = true;
        currentStamina = maxStamina;
        input = new Action();
    }

    private void OnEnable() => input.Player.Enable();
    private void OnDisable() => input.Player.Disable();

    private void Update()
    {
        // --- Input ---
        Vector2 rawInput = input.Player.Move.ReadValue<Vector2>();
        isRunning = input.Player.Run.IsPressed();
        // tru stamina khi chay
        if (isRunning)
        {
            currentStamina = Mathf.Clamp(currentStamina - staminaSubAmount, 0, maxStamina);
        }

        smoothedInput = Vector2.Lerp(smoothedInput, rawInput, Time.deltaTime * 10f);
        moveInput = smoothedInput;

        // --- Hướng di chuyển theo camera ---
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        moveDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        currentSpeed = isRunning ? runSpeed : walkSpeed;
        // stamina thap thi khong chay nhanh duoc
        if (currentStamina <= staminaThreshold)
        {
            currentSpeed = walkSpeed;
            isRunning = false;
        }

        animController.UpdateMovement(moveInput, isRunning);

        /*
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            // Xác định góc xoay mong muốn dựa theo hướng input và hướng camera
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

            // Giữ góc xoay mượt
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * 10f);

            // Cập nhật rotation theo trục Y (xoay quanh mặt đất)
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
        }
        */
        // --- Nhận lệnh nhảy: chỉ trigger animation, không apply lực ngay nếu dùng animation event ---
        if (input.Player.Jump.triggered && IsGrounded())
        {
            if (animController.CanPlayJump())
            {
                animController.PlayJump();

                if (!useAnimationEvent)
                {
                    // fallback: gọi Jump sau delay nhỏ để khớp với anim
                    if (jumpCoroutine != null) StopCoroutine(jumpCoroutine);
                    jumpCoroutine = StartCoroutine(DelayedJumpRoutine(jumpDelay));
                }
                // nếu useAnimationEvent==true thì animation clip sẽ gọi OnJumpAnimationEvent()
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = moveDirection * currentSpeed;
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = Vector3.SmoothDamp(
            rb.linearVelocity,
            targetVelocity,
            ref currentVelocity,
            moveDirection.sqrMagnitude > 0.01f ? 1f / acceleration : 1f / deceleration
        );

        // Nếu tốc độ ngang quá nhỏ thì dừng hẳn
        Vector3 horizontal = rb.linearVelocity;
        horizontal.y = 0f;
        if (horizontal.magnitude < 0.1f)
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);

        // 🔹 Kiểm tra chuyển trạng thái từ trên không -> chạm đất
        bool grounded = IsGrounded();
        if (grounded && !wasGroundedLastFrame)
        {
            // Vừa tiếp đất xong
            animController.EndJump();  // 🔸 Gọi hàm reset anim Jump
        }

        wasGroundedLastFrame = grounded;
    }



    private IEnumerator DelayedJumpRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (IsGrounded()) Jump();
        jumpCoroutine = null;
    }

    // --- Gọi bởi Animation Event (tên phải trùng với event trong clip) ---
    public void OnJumpAnimationEvent()
    {
        if (IsGrounded()) Jump();
    }

    private void Jump()
    {
        // Lấy hướng ngang hiện tại
        Vector3 horizontalVelocity = rb.linearVelocity;
        horizontalVelocity.y = 0f;

        Vector3 jumpDir = horizontalVelocity.sqrMagnitude > 0.1f ? horizontalVelocity.normalized : moveDirection;

        // Reset Y trước khi apply lực
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;

        Vector3 jumpForceVector = Vector3.up * jumpForce;
        if (jumpDir.sqrMagnitude > 0.01f)
            jumpForceVector += jumpDir * (jumpForce * jumpForwardMultiplier);

        rb.AddForce(jumpForceVector, ForceMode.VelocityChange);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.3f);
    }

    // --- Nếu bật Apply Root Motion trên Animator thì dùng OnAnimatorMove() để dịch chuyển Rigidbody theo root motion ---
    private void OnAnimatorMove()
    {
        if (animator != null && animator.applyRootMotion)
        {
            // Dùng MovePosition để tương tác đúng với Rigidbody
            rb.MovePosition(rb.position + animator.deltaPosition);
        }
    }
    public void AddStamina(float value)
    {
        currentStamina = Mathf.Clamp(currentStamina + value, 0, maxStamina);
    }
}
