using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance;

    [Header("Thời gian giới hạn (giây)")]
    public float totalTime = 60f;

    [Header("Text hiển thị thời gian còn lại")]
    [SerializeField] private TMP_Text timerText;

    [Header("UI hiển thị khi thắng (đã lật hết bài)")]
    [SerializeField] private GameObject successUI;

    [Header("UI hiển thị khi thua (hết giờ)")]
    [SerializeField] private GameObject failureUI;

    [Header("Button leave")]
    [SerializeField] private GameObject leaveButton;

    // Thêm ScriptableObject quest của mini game vào đây
    [SerializeField] private QuestInfoSO questInfo;



    private float remainingTime;
    private bool timerRunning;
    private bool gameEnded;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartTimer();
    }

    public void StartTimer()
    {
        remainingTime = totalTime;
        timerRunning = true;
        gameEnded = false;

        if (successUI) successUI.SetActive(false);
        if (failureUI) failureUI.SetActive(false);
    }

    private void Update()
    {
        if (!timerRunning || gameEnded)
            return;

        // 🔥 QUAN TRỌNG: dùng unscaledDeltaTime
        remainingTime -= Time.unscaledDeltaTime;
        UpdateTimerUI();

        // Thắng
        if (CardManager.Instance != null && AllCardsMatched())
        {
            EndGame(true);
            return;
        }

        // Thua
        if (remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame(false);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            timerText.text = $"{seconds}s";
        }
    }

    private bool AllCardsMatched()
    {
        var allCards = FindObjectsByType<CardFlipEffect>(FindObjectsSortMode.None);

        foreach (var card in allCards)
        {
            var field = typeof(CardFlipEffect)
                .GetField("matched",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);

            bool isMatched = (bool)field.GetValue(card);

            if (!isMatched)
                return false;
        }

        return true;
    }

    public void EndGame(bool allMatched)
    {
        if (gameEnded) return;

        timerRunning = false;
        gameEnded = true;

        if (allMatched)
        {
            Debug.Log("🎉 Thắng! Nhận 200 xu");
            GameManager_.Instance.GetCurrencyManager().AddCash(200);

            SceneManager_.Instance.ExitAdditiveScene("CardFlip");
            AudioManager.Instance.PlayMusic("background1");
        }
        else
        {
            Debug.Log("⌛ Hết giờ! Nhận 50 xu");
            GameManager_.Instance.GetCurrencyManager().AddCash(50);

            if (failureUI)
            {
                failureUI.SetActive(true);
                leaveButton.SetActive(true);
            }
        }
    }

}
