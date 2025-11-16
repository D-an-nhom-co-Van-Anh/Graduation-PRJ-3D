using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;

    [Header("Player Model")]
    public GameObject playerModel;

    private bool isFirstPerson = false;

    public void SetFirstPerson(bool value)
    {
        isFirstPerson = value;
        SwitchCamera();
    }

    public void ToggleCamera()
    {
        isFirstPerson = !isFirstPerson;
        SwitchCamera();
    }

    private void SwitchCamera()
    {
        if (isFirstPerson)
        {
            firstPersonCam.Priority = 10;
            thirdPersonCam.Priority = 0;
                playerModel.SetActive(false);
        }
        else
        {
            firstPersonCam.Priority = 0;
            thirdPersonCam.Priority = 10;
                playerModel.SetActive(true);
        }
    }

    public bool IsFirstPersonView()
    {
        return isFirstPerson;
    }
}
