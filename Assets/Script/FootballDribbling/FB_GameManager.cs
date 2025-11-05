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

    public float timeOut = 0;
    public float maxTimeOut = 15;
    [SerializeField] private GameObject failCanvas;
    [SerializeField] private TextMeshProUGUI myText;
    [SerializeField] private Quest quest;
    [SerializeField] private FB_GoalKeeper keeper;
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
        player.transform.DOMove(penaltySpot.position, 0.97f).OnComplete(() =>{
            targetPoint.SetActive(true);
            Debug.Log("ket thuc");
            player.GetPlayerController().PlayMoveAnimation(Vector2.zero);
            // Chuẩn bị trạng thái penalty
            player.EnterPenaltyMode(targetPoint, ball);

        });
        player.transform.DOLookAt(penaltySpot.position, 0.1f, AxisConstraint.Y);
        player.transform.rotation = penaltySpot.rotation;
        Debug.Log(penaltySpot.position);
        Debug.Log(player.transform.position);
        // Hiện mục tiêu khung thành
     
    }
    private bool isOver;
    public bool IsOver => isOver;
    private void Start()
    {
        Cursor.visible = false;
        currentTime = totalTime;
        messageText.text = "";
        Time.timeScale = 1f;
        isOver = false;
    }

    private void Update()
    {
        if (isGameOver) return;

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
    private void GameOver(bool success)
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
