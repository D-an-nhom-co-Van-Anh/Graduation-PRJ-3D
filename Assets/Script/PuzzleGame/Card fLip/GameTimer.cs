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
    private bool timerRunning = false;
    private bool gameEnded = false;

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

        // Ẩn UI kết quả khi bắt đầu
        if (successUI) successUI.SetActive(false);
        if (failureUI) failureUI.SetActive(false);

        StartCoroutine(TimerCountdown());
    }

    private IEnumerator TimerCountdown()
    {
        while (timerRunning && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
            yield return null;

            // Nếu tất cả các thẻ đã lật hết -> thắng
            if (CardManager.Instance != null && AllCardsMatched())
            {
                EndGame(true);
                yield break;
            }
        }

        // Hết giờ mà chưa thắng
        if (!gameEnded)
            EndGame(false);
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
        var allCards = FindObjectsOfType<CardFlipEffect>();
        foreach (var card in allCards)
        {
            var field = typeof(CardFlipEffect).GetField("matched", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isMatched = (bool)field.GetValue(card);
            if (!isMatched)
                return false;
        }
        return true;
    }

    public void EndGame(bool allMatched)
    {
        if (gameEnded) return; // tránh gọi nhiều lần

        timerRunning = false;
        gameEnded = true;

        if (allMatched)
        {
            if (successUI)
            {
                successUI.SetActive(true);
                leaveButton.SetActive(true);
            }

                Debug.Log("🎉 Thắng! Nhận 200 xu");

            GameManager_.Instance.GetCurrencyManager().AddCash(200);
            //Khi nào ghép hoàn thị game thì bỏ commment -> cập nhật trạng thái quest
            GameManager_.Instance.GetQuestManager().FinishQuest(questInfo.id);
            // TODO: PlayerData.AddCoins(200);
        }
        else
        {
            if (failureUI)
            {
                failureUI.SetActive(true);
                leaveButton.SetActive(true);
            }
            Debug.Log("⌛ Hết giờ! Nhận 50 xu");
            GameManager_.Instance.GetCurrencyManager().AddCash(50);
            // TODO: PlayerData.AddCoins(50);
        }
    }

}
