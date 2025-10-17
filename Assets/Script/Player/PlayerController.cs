using Unity.Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputSystem_Actions input;
    [SerializeField] private float rotation = 10f;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float acceleration = 10f;   
    [SerializeField] private float deceleration = 10f;   
    private Vector3 currentVelocity; // dung cho SmoothDamp
    private Vector2 moveDir;
    private Vector3 dir;
    private InteractableObj current_InteractableObj;
    private Vector3 camForward;
    private Vector3 camRight;
    private Vector3 targetVelocity;
    private Vector3 smoothVelocity;
    private Vector3 flatVel;
    void Start()
    {
        input = new InputSystem_Actions();
        input.Player.Enable();
        input.Player.Interact.performed += Interact_performed;
        dir = Vector3.zero;
    }
    private void OnDestroy()
    {
        input.Player.Interact.performed -= Interact_performed;
        input.Disable();
    }
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log(current_InteractableObj);
        if (current_InteractableObj != null)
        {
            current_InteractableObj.Interact();
        }
    }

    void Update()
    {
        moveDir = input.Player.Move.ReadValue<Vector2>().normalized;
        camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();
        camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();
      
        // Chuyen input WASD sang world space dua tren camera
        dir = camForward * moveDir.y + camRight * moveDir.x;
    }
    private void FixedUpdate()
    {
        // Tinh van toc muc tieu
        targetVelocity = dir * speed;

        // Giu lai gravity
        targetVelocity.y = rb.linearVelocity.y;

        // Dung SmoothDamp de tang/giam toc
        smoothVelocity = Vector3.SmoothDamp(rb.linearVelocity, targetVelocity, ref currentVelocity,
            dir.sqrMagnitude > 0.01f ? 1f / acceleration : 1f / deceleration);

        rb.linearVelocity = smoothVelocity;

        // Xoay nhan vat theo huong di chuyen (neu co input)
        flatVel.x = rb.linearVelocity.x;
        flatVel.z = rb.linearVelocity.z;
        flatVel.y = 0; 
        if (flatVel.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatVel, Vector3.up);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, rotation * Time.fixedDeltaTime));
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        InteractableObj obj = other.GetComponent<InteractableObj>();
        if (obj!=null)
        {
            current_InteractableObj = obj;

            // -> hien UI 
        }
    }
    public void TestQuest()
    {
        GameManager_.Instance.GetQuestManager().FinishQuest("Test2");
    }
}
