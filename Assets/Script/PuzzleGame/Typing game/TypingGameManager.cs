using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class TypingGameManager : MonoBehaviour
{
    #region === UI References ===
    [Header("UI References")]
    [Tooltip("Text hiển thị từ cần gõ")]
    public TMP_Text wordDisplay;

    [Tooltip("Text hiển thị tiến độ (vd: Đúng: 3/10 | Sai: 1)")]
    public TMP_Text progressText;

    [Tooltip("Script hiệu ứng chữ bay")]
    public TypingEffects typingEffect;
    #endregion

    #region === Gameplay Settings ===
    [Header("Gameplay Settings")]
    [Tooltip("Danh sách các từ có thể xuất hiện")]
    private List<string> wordList = new List<string>()
    {
        "SPRITE", "PIXEL", "ANIMATION", "CANVAS", "SHADER", "TEXTURE", "LIGHTING", "MATERIAL",
        "COLOR", "ENVIRONMENT", "VISUAL", "ASSET", "LAYER", "OVERLAY", "VFX", "UI", "HUD",
        "SCRIPT", "UPDATE", "FUNCTION", "VARIABLE", "COMPONENT", "OBJECT", "TRANSFORM",
        "POSITION", "ROTATION", "SCALE", "TRIGGER", "COLLIDER", "PHYSICS", "RIGIDBODY",
        "PLAYER", "ENEMY", "LEVEL", "MISSION", "QUEST", "HEALTH", "SCORE", "POWERUP",
        "ITEM", "WEAPON", "DAMAGE", "SPAWN", "UNITY", "BUILD"
    };

    private string currentWord;
    private int currentIndex = 0;

    // Sự kiện cho các script khác (như TypingTimerAndReward)
    public System.Action OnWordCompleted;
    public System.Action OnNewWord;
    public System.Action OnWordFailed; // Khi hết thời gian mà chưa hoàn thành
    #endregion

    #region === Round Settings ===
    [Header("Round Settings")]
    [Tooltip("Số từ cần gõ trong 1 lượt chơi")]
    public int maxWordsPerRound = 10;

    private int wordsCompleted = 0; // số từ đúng
    private int failedWords = 0;    // số từ bị fail
    private bool roundEnded = false;
    #endregion

    //them cai scriptableObject chứa nhiem vu vao day để lấy id ->gọi khi hoàn thành quest
    [SerializeField] private QuestInfoSO questInfo;
    #region === Unity Lifecycle ===
    void Start()
    {
        AudioManager.Instance.StopMusic();
        PickNewWord();
        UpdateProgressUI();
    }

    void Update()
    {
        if (roundEnded) return;

        foreach (char c in Input.inputString)
        {
            HandleInput(c);
        }
    }
    #endregion


    #region === Input Handling ===
    /// <summary>
    /// Kiểm tra ký tự người chơi nhập và xử lý kết quả.
    /// </summary>
    void HandleInput(char inputChar)
    {
        if (string.IsNullOrEmpty(currentWord)) return;

        inputChar = char.ToUpper(inputChar);

        // Nếu gõ đúng ký tự
        if (inputChar == currentWord[currentIndex])
        {
            Vector3 charWorldPos = GetCharacterWorldPosition(currentIndex);
            typingEffect.SpawnFlyingLetter(inputChar, charWorldPos);
            currentIndex++;

            // Nếu hoàn thành từ
            if (currentIndex >= currentWord.Length)
            {
                typingEffect.PlayWordCompleteEffect();
                OnWordCompleted?.Invoke();

                wordsCompleted++;
                UpdateProgressUI();

                if (TotalWordsPlayed() >= maxWordsPerRound)
                    EndRound();
                else
                    Invoke(nameof(PickNewWord), 0.5f);
            }
            else
            {
                UpdateWordDisplay();
            }
        }
        else
        {
            typingEffect.PlayWrongLetterEffect(wordDisplay.transform);
        }
    }
    #endregion


    #region === Word Management ===
    /// <summary>
    /// Chọn từ mới ngẫu nhiên trong danh sách.
    /// </summary>
    void PickNewWord()
    {
        if (roundEnded) return;

        currentWord = wordList[Random.Range(0, wordList.Count)];
        currentIndex = 0;
        UpdateWordDisplay();

        OnNewWord?.Invoke();
    }

    /// <summary>
    /// Được gọi khi hết thời gian gõ 1 từ (do TypingTimerAndReward gọi).
    /// </summary>
    public void ForceNextWord()
    {
        if (roundEnded) return;

        // Nếu người chơi chưa hoàn thành từ thì tính là "fail"
        if (currentIndex < currentWord.Length)
        {
            failedWords++;
            OnWordFailed?.Invoke();
            typingEffect.PlayWrongLetterEffect(wordDisplay.transform);
            Debug.Log($"Time Out! Từ '{currentWord}' bị bỏ lỡ. Fail: {failedWords}");
        }

        UpdateProgressUI();

        // Kiểm tra nếu đã hết tổng số từ
        if (TotalWordsPlayed() >= maxWordsPerRound)
            EndRound();
        else
            PickNewWord();
    }

    /// <summary>
    /// Tổng số từ đã chơi (bao gồm đúng và sai).
    /// </summary>
    int TotalWordsPlayed()
    {
        return wordsCompleted + failedWords;
    }
    #endregion


    #region === Round Management ===
    /// <summary>
    /// Kết thúc vòng chơi (đã gõ đủ 10 từ hoặc hết lượt).
    /// </summary>
    void EndRound()
    {
        roundEnded = true;
        wordDisplay.text = "<color=#FFD700> Finish Round! </color>";

        if (progressText != null)
            progressText.text = $"Result: <color=#00FF7F>{wordsCompleted}</color> / {maxWordsPerRound} Correct" +
                                $"<color=#FF5555>{failedWords}</color> incorrect";

        // khi nao ghép vào game hoàn chỉnh thì bỏ commment-> dùng để end quest với cập nhật trạng thái quest
        StartCoroutine(ExitTypingAfterDelay(3f));
        Debug.Log($" Round completed! {wordsCompleted}/{maxWordsPerRound} correct, {failedWords} incorrect.");
    }
    #endregion


    #region === UI Updates ===
    /// <summary>
    /// Cập nhật hiển thị từ đang gõ (phần đã gõ có màu).
    /// </summary>
    void UpdateWordDisplay()
    {
        if (currentWord == null) return;

        string typedPart = $"<color=#00FF7F>{currentWord.Substring(0, currentIndex)}</color>";
        string remainingPart = currentWord.Substring(currentIndex);
        wordDisplay.text = typedPart + remainingPart;
    }

    /// <summary>
    /// Cập nhật thanh tiến độ "Đúng/Sai".
    /// </summary>
    void UpdateProgressUI()
    {
        if (progressText != null)
            progressText.text = $"Correct: {wordsCompleted}/{maxWordsPerRound} | Incorrect: {failedWords}";
    }
    #endregion
    IEnumerator ExitTypingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager_.Instance.ExitAdditiveScene("Typing");
        AudioManager.Instance.PlayMusic("background1");

        Debug.Log(" Exited Typing scene after delay");
    }

    #region === Utility Functions ===
    /// <summary>
    /// Lấy vị trí thế giới của ký tự trong TMP_Text.
    /// </summary>
    Vector3 GetCharacterWorldPosition(int index)
    {
        if (wordDisplay.textInfo == null || index < 0 || index >= wordDisplay.textInfo.characterCount)
            return wordDisplay.transform.position;

        var charInfo = wordDisplay.textInfo.characterInfo[index];
        if (!charInfo.isVisible)
            return wordDisplay.transform.position;

        Vector3 midPoint = (charInfo.topLeft + charInfo.bottomRight) / 2f;
        Vector3 worldPos = wordDisplay.transform.TransformPoint(midPoint);
        return worldPos;
    }
    #endregion
}
