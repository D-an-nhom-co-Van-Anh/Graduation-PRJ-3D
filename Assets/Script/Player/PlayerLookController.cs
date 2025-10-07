using UnityEngine;

public class PlayerLookController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody rb;
    private PlayerMovementController movementController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movementController = GetComponent<PlayerMovementController>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = movementController.MoveDirection;

        // Nếu đang di chuyển thì xoay theo hướng
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotationSpeed * Time.fixedDeltaTime));
        }
    }
}
