using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputSystem_Actions input;
    [SerializeField] private float rotation = 10f;
    [SerializeField] private float speed = 10f;
    private Vector3 dir;
    private InteractableObj current_InteractableObj;

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
    }
    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log(current_InteractableObj);
        if (current_InteractableObj != null)
        {
            current_InteractableObj.Interact();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveDir = input.Player.Move.ReadValue<Vector2>().normalized;
        dir.x = moveDir.x;
        dir.z = moveDir.y;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = Vector3.Slerp(transform.forward, dir, Time.deltaTime * rotation);
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
}
