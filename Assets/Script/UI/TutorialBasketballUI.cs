using System;
using Unity.Cinemachine;
using UnityEngine;

public class TutorialBasketballUI:UICanvas
{
    private const string TutorialKey = "HasSeenBasketballTutorial";
    private GameObject cinemachineCamera;
    private PlayerFPSController _fpsController;

    public override void Open()
    {   
        cinemachineCamera= GameObject.FindWithTag("BasketballCameraPlayer");
        _fpsController=cinemachineCamera.GetComponent<PlayerFPSController>();
        _fpsController.enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (PlayerPrefs.GetInt(TutorialKey, 0) == 1)
        {
            
            return;
        }
        base.Open();
    }

    public override void CloseDirect()
    {
        _fpsController.enabled = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerPrefs.SetInt(TutorialKey, 1);
        PlayerPrefs.Save();
        base.CloseDirect();
    }
}
