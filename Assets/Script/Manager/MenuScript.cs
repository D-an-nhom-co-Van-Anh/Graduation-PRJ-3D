using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;
using TMPro;
public class MenuScript : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private TextMeshProUGUI titleGame;
    [SerializeField] private GameObject cutscene;
    private PlayerMovementController player;
    private static string PLAY_CUTSCENE = "Play_Cutscene";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        titleGame.rectTransform.DOLocalMove(new Vector3(0, 214, 0), 1f).SetEase(Ease.InBack);
        player = GameManager_.Instance.GetPlayer();
        player.LockMovement();
        playButton.onClick.AddListener(() => {
            StartCoroutine(PlayCutScene());
        });
    }
    IEnumerator PlayCutScene()
    {
        if (PlayerPrefs.GetInt(PLAY_CUTSCENE, 0) == 0)
        {
            cutscene.SetActive(true);
            PlayerPrefs.SetInt(PLAY_CUTSCENE, 1);
        }
            player.UnlockMovement();
        yield return new WaitForSeconds(0.1f);
        this.gameObject.SetActive(false);
    }
    private void Update()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
