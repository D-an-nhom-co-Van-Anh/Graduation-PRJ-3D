using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFPSController : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float sensitivity = 200f;

    [Header("Rotation Limits")]
    public float verticalClamp = 80f;

    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start()
    {
        // Kh�a chu?t v�o gi?a m�n h�nh
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // L?y input chu?t t? Input System
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * sensitivity * Time.deltaTime;

        // Xoay l�n/xu?ng (tr?c X)
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        // Xoay tr�i/ph?i (tr?c Y)
        yRotation += mouseDelta.x;

        // G�n rotation cho camera (ch�nh object n�y)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
