using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;

    [Header("Player Model (the visible mesh)")]
    public GameObject playerModel;

    private bool isFirstPerson = false;

    void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            isFirstPerson = !isFirstPerson;
            SwitchCamera();
        }
    }

    void SwitchCamera()
    {
        if (isFirstPerson)
        {
            firstPersonCam.Priority = 10;
            thirdPersonCam.Priority = 0;

            if (playerModel != null)
                playerModel.SetActive(false);
        }
        else
        {
            firstPersonCam.Priority = 0;
            thirdPersonCam.Priority = 10;

            if (playerModel != null)
                playerModel.SetActive(true);
        }
    }
}
