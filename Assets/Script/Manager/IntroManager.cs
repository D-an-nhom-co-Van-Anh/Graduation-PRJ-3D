using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
public class IntroManager : MonoBehaviour
{
    PlayerMovementController player;
    [SerializeField] private CinemachineCamera camera1;
    [SerializeField] private CinemachineCamera camera2; 
    [SerializeField] private CinemachineCamera camera3; 

    private void Awake()
    {
        camera1.gameObject.SetActive(true);
        camera2.gameObject.SetActive(false);
        camera3.gameObject.SetActive(false);
        player = GameManager_.Instance.GetPlayer();
        player.GetComponentInChildren<SkinnedMeshRenderer>().enabled=false;
    }
    public void ChangeCamera()
    {
        StartCoroutine(ChangeCameraCoroutine());
    }
    public IEnumerator ChangeCameraCoroutine()
    {
        camera1.gameObject.SetActive(false);
        camera2.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        player.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        yield return new WaitForSeconds(10f);
        camera2.gameObject.SetActive(false);
        camera3.gameObject.SetActive(true);
        Cursor.visible = false;
        yield return new WaitForSeconds(2f);
        GameManager_.Instance.StartGame();
    }
}
