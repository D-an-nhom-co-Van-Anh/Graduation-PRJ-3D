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
        //PlayerPrefs.DeleteKey(PLAY_CUTSCENE);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //titleGame.rectTransform.DOLocalMove(new Vector3(0, 214, 0), 1f).SetEase(Ease.InBack);
        Sequence seq = DOTween.Sequence();

        seq.Append(
            titleGame.rectTransform
                .DOLocalMove(new Vector3(0, 214, 0), 1.1f)
                .SetEase(Ease.OutBack)
        );

        seq.Join(
            titleGame.rectTransform
                .DOScale(1f, 0.9f)
                .From(0.7f)
                .SetEase(Ease.OutBack)
        );
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
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            this.gameObject.SetActive(false);
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
