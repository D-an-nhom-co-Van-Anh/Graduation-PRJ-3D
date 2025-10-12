using UnityEngine;

[RequireComponent(typeof(PlayerMovementController))]
public class PlayerLookController : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 10f;

    private PlayerMovementController movementController;
    private Rigidbody rb;

    private void Awake()
    {
        movementController = GetComponent<PlayerMovementController>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 moveDir = movementController.moveDirection;

        // Nếu đang di chuyển thì xoay nhân vật theo hướng di chuyển
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            Quaternion smoothRotation = Quaternion.Slerp(
                rb.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(smoothRotation);
        }
    }
}
