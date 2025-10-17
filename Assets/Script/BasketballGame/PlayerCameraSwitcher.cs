using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PlayerCameraSwitcher : MonoBehaviour
{
    [Header("Camera References")]
    public CinemachineCamera firstPersonCam;
    public CinemachineCamera thirdPersonCam;

    [Header("Player Model (the visible mesh)")]
    public GameObject playerModel;
    private ThrowBasketball throwBasketballScript;
    private bool isFirstPerson = false;
    private void Start()
    {
        GameObject basketball = GameObject.Find("basketball");
        throwBasketballScript = basketball.GetComponent<ThrowBasketball>();
        
    }
    void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame)
        {
            isFirstPerson = !isFirstPerson;
            throwBasketballScript.ResetBallPosition();
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

    public bool IsFirstPersonView()
    {
        return isFirstPerson;
    }
}
