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
        // Khóa chu?t vào gi?a màn hình
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // L?y input chu?t t? Input System
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * sensitivity * Time.deltaTime;

        // Xoay lên/xu?ng (tr?c X)
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        // Xoay trái/ph?i (tr?c Y)
        yRotation += mouseDelta.x;

        // Gán rotation cho camera (chính object này)
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
