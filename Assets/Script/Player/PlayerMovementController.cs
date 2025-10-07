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
    [SerializeField] private Transform cameraTransform;

    private Rigidbody rb;
    private PlayerAnimationController animController;
    private InputSystem_Actions input;

    private Vector2 moveInput;
    public Vector3 MoveDirection;
    private Vector3 currentVelocity;
    private bool isRunning;
    private float currentSpeed;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animController = GetComponent<PlayerAnimationController>();
        input = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    private void Update()
    {
        // --- Lấy input ---
        moveInput = input.Player.Move.ReadValue<Vector2>();
        isRunning = input.Player.Run.IsPressed();

        // --- Tính hướng theo camera ---
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        MoveDirection = camForward * moveInput.y + camRight * moveInput.x;

        // --- Tính tốc độ ---
        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // --- Cập nhật animation ---
        animController.UpdateMovement(moveInput, isRunning);

        // --- Nhảy ---
        if (input.Player.Jump.triggered)
        {
            animController.PlayJump();
        }
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = MoveDirection * currentSpeed;
        targetVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = Vector3.SmoothDamp(
            rb.linearVelocity,
            targetVelocity,
            ref currentVelocity,
            MoveDirection.sqrMagnitude > 0.01f ? 1f / acceleration : 1f / deceleration
        );
    }
}
