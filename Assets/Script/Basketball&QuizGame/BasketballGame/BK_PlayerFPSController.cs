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


    void Update()
    {
        SettingFirstPerson();
    }
    void SettingFirstPerson()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue() * sensitivity * Time.deltaTime;

        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        yRotation += mouseDelta.x;

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
