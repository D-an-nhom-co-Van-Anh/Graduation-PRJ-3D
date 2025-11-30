using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
public class FB_GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI messageText;

    public float totalTime = 30f;
    private float currentTime;
    private int score;
    private bool isGameOver = false;
    private bool isGameStart = false;
    public bool IsGameStart => isGameStart;
    public float timeOut = 0;
    public float maxTimeOut = 15;
    [SerializeField] private GameObject failCanvas;
    [SerializeField] private TextMeshProUGUI myText;
    [SerializeField] private QuestInfoSO quest;
    [SerializeField] private FB_GoalKeeper keeper;
    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private Image fadeImage;
    [SerializeField] private GameObject fb_canvas;
    public Transform penaltySpot; // vị trí sút penalty
    public CinemachineCamera firstPersonCamera;
    public CinemachineCamera thirdPersonCamera;
    public GameObject targetPoint; // điểm trong khung thành
    public FB_Ball ball;
    private PlayerController playerController;
    public void StartPenaltyMode(FB_PlayerController player)
    {
        this.playerController = player.GetPlayerController();
        StartCoroutine(StartPenaltySequence(player));
    }
    private IEnumerator StartPenaltySequence(FB_PlayerController player)
    {
        fadeImage.gameObject.SetActive(true);
        player.GetPlayerController().LockMovement();

        yield return fadeImage.DOFade(1f, 0.6f).WaitForCompletion();

        thirdPersonCamera.gameObject.SetActive(false);
        firstPersonCamera.gameObject.SetActive(true);

        player.transform.position = penaltySpot.position;
        player.GetPlayerController().PlayMoveAnimation(Vector2.zero);
        player.transform.rotation = Quaternion.LookRotation(
            targetPoint.transform.position - penaltySpot.position, Vector3.up);


        yield return fadeImage.DOFade(0f, 0.6f).WaitForCompletion();
        player.EnterPenaltyMode(targetPoint, ball);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1f);
        ball.StartPenalty();
        fadeImage.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        isGameStart = true;
        Cursor.visible = false;
        instructionPanel.SetActive(false);

        timerText.text = $"{Mathf.Ceil(currentTime)}";
        //scoreText.text = $"{score}";
    }
    public void ExitPenaltyMode()
    {
        firstPersonCamera.gameObject.SetActive(false);
        thirdPersonCamera.gameObject.SetActive(true);

    }
    private bool isOver;
    public bool IsOver => isOver;
    private void Start()
    {
        Cursor.visible = true;
        currentTime = totalTime;
        //messageText.text = "";
        Time.timeScale = 1f;
        StartCoroutine(FadeImage());
        isOver = false;
    }
    public IEnumerator DelayInstruction()
    {
        fb_canvas.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.57f);
        fb_canvas.gameObject.SetActive(true);
    }
    public IEnumerator FadeImage()
    {
        fadeImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.57f);
        fadeImage.gameObject.SetActive(false);
    }
    private void Update()
    {
        if (isGameOver) return;
        if (!isGameStart) return;
        currentTime -= Time.deltaTime;
        timerText.SetText($"{Mathf.Ceil(currentTime)}");
        if (currentTime <= 0)
        {
            GameOver(false);
        }
    }
    public FB_GoalKeeper GetKeeper()
    {
        return keeper;
    }
    public void OnHitObstacle()
    {
        GameOver(false);
    }

    public void OnReachGoal()
    {
        GameOver(true);
    }
    public void ChangeCamera()
    {

    }
    public void GameOver(bool success)
    {
        if (isGameOver != true)
        {
            isGameOver = true;
            if (success)
            {
               // messageText.text = "Hoàn thành! Bạn nhận được CUP!";
                playerController.PlayVictory();
                GameManager_.Instance.GetQuestManager().FinishQuest("Quest6Info");
                Invoke(nameof(ChangeScene),1f);
            }
            else
            {
                isOver = true;
                Cursor.visible = true;
                failCanvas.SetActive(true);
                ShowFailCanvasText();
            }
        }
    }
    public void ChangeScene()
    {
        GameManager_.Instance.GetMainCanvas().SetActive(true);
        GameManager_.Instance.GetPlayer().UnlockMovement();
        SceneManager_.Instance.ExitAdditiveScene("Football");
    }
    public void Restart()
    {
        SceneManager_.Instance.ReloadAdditiveScene("Football");
    }
   
    private void ShowFailCanvasText()
    {
        
        RectTransform rect = myText.GetComponent<RectTransform>();

        // Tween phóng to rồi thu nhỏ lại
        rect.DOScale(1.5f, 0.3f) // phóng to lên 1.5 lần trong 0.3 giây
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                rect.DOScale(1f, 0.3f).SetEase(Ease.InBack); // thu nhỏ về 1
            });
    }
}
