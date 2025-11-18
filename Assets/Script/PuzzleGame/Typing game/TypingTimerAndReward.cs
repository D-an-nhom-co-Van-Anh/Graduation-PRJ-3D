using UnityEngine;
using TMPro;

public class TypingTimerAndReward : MonoBehaviour
{
    [Header("References")]
    public TypingGameManager typingGameManager;
    public TMP_Text timeText;
    public TMP_Text moneyText;

    [Header("Settings")]
    public float maxTimePerWord = 15f;
    public int rewardPerWord = 200;

    private float timer;
    private int totalMoney;
    private bool isTiming = false;

    void Start()
    {
        timer = maxTimePerWord;
        UpdateUI();

        typingGameManager.OnWordCompleted += HandleWordCompleted;
        typingGameManager.OnNewWord += HandleNewWord;

        // ✅ Nếu game đã khởi tạo từ rồi, tự bật timer cho từ đầu tiên
        HandleNewWord();
    }

    void Update()
    {
        if (!isTiming) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            timer = 0;
            isTiming = false;
            typingGameManager.ForceNextWord();
        }

        UpdateUI();
    }

    void HandleWordCompleted()
    {
        totalMoney += rewardPerWord;
        GameManager_.Instance.GetCurrencyManager().AddCash(rewardPerWord);
        isTiming = false;
        UpdateUI();
    }

    void HandleNewWord()
    {
        timer = maxTimePerWord;
        isTiming = true;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (timeText != null)
            timeText.text = $"Time: {timer:F1}s";

        if (moneyText != null)
            moneyText.text = $"Money: {totalMoney}";
    }
}
