using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.Cinemachine;

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
    [SerializeField] private Quest quest;
    [SerializeField] private FB_GoalKeeper keeper;
    [SerializeField] private GameObject instructionPanel;
    public Transform penaltySpot; // vị trí sút penalty
    public CinemachineCamera firstPersonCamera;
    public CinemachineCamera thirdPersonCamera;
    public GameObject targetPoint; // điểm trong khung thành
    public FB_Ball ball;
    public void StartPenaltyMode(FB_PlayerController player)
    {
        // Tắt camera góc 3
        thirdPersonCamera.gameObject.SetActive(false);
        // Bật camera góc 1
        firstPersonCamera.gameObject.SetActive(true);

        player.GetPlayerController().LockMovement();
        // Dịch nhân vật tới chỗ sút
        player.GetPlayerController().PlayMoveAnimation(Vector2.one);
        player.transform.DOLookAt(penaltySpot.position, 0.2f, AxisConstraint.Y).OnComplete(() => Debug.Log("Quay"));
        player.transform.DOMove(penaltySpot.position, 1.97f).OnComplete(() =>{
            //targetPoint.SetActive(true);
            Debug.Log("ket thuc");
            player.GetPlayerController().PlayMoveAnimation(Vector2.zero);
            // Chuẩn bị trạng thái penalty
            player.EnterPenaltyMode(targetPoint, ball);
            Cursor.visible = true;
            player.transform.DOLookAt(targetPoint.transform.position, 0.2f, AxisConstraint.Y);
            ball.StartPenalty();
        });
      

        player.transform.rotation = penaltySpot.rotation;

     
    }
    public void StartGame()
    {
        isGameStart = true;
        Cursor.visible = false;
        instructionPanel.SetActive(false);
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
        messageText.text = "";
        Time.timeScale = 1f;
        isOver = false;
    }

    private void Update()
    {
        if (isGameOver) return;
        if (!isGameStart) return;
        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            GameOver(false);
        }

        timerText.text = $"{Mathf.Ceil(currentTime)}";
        scoreText.text = $"{score}";
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
                messageText.text = "Hoàn thành! Bạn nhận được CUP!";
                //GameManager_.Instance.GetQuestManager().FinishQuest(quest.info.id);
                // LoadSceneCoroutine(SceneManager.GetSceneAt(0).name);
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
    public void Restart()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    private IEnumerator LoadSceneCoroutine(string sceneName, float progress = 0)
    {
        // Start loading the scene asynchronously
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        // Check if the scene exists and the loading operation was started successfully
        if (asyncOperation == null)
        {
            yield break;
        }

        // Disable automatic scene activation to manage progress and events
        asyncOperation.allowSceneActivation = false;

        // Track loading progress
        while (!asyncOperation.isDone)
        {
            // Report progress (0.0 to 0.9 during the loading phase)
            progress = Mathf.Clamp01(asyncOperation.progress / 0.9f);
            Debug.Log($"Progress: {progress}");
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true; // Activate the scene
            }
            if (timeOut >= maxTimeOut)
            {
                SceneManager.LoadScene(sceneName);
                Debug.LogWarning("Time out");
                yield break;
            }

            yield return null; // Wait for the next frame
        }
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
