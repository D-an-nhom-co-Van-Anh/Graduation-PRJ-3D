using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TypingGameFull : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI wordToTypeText;   // Hiển thị từ cần gõ
    public TextMeshProUGUI currentInputText; // Hiển thị từ người chơi đang nhập
    public TextMeshProUGUI scoreText;        // Hiển thị điểm
    public TextMeshProUGUI endGameText;      // Hiển thị khi game kết thúc

    [Header("Game Settings")]
    public int maxScore = 10;        // Số điểm tối đa để thắng
    public bool useUppercase = true; // Dùng chữ in hoa hay không

    private string currentWord;
    private string typedWord = "";
    private int score = 0;
    private bool gameOver = false;

    private HashSet<string> usedWords = new HashSet<string>();

    // 🎮 Danh sách từ liên quan đến phát triển game
    private List<string> gameWords = new List<string>()
    {
        "SPRITE", "PIXEL", "ANIMATION", "CANVAS", "SHADER", "TEXTURE", "LIGHTING", "MATERIAL",
        "COLOR", "ENVIRONMENT", "VISUAL", "ASSET", "LAYER", "OVERLAY", "VFX", "UI", "HUD",
        "SCRIPT", "UPDATE", "FUNCTION", "VARIABLE", "COMPONENT", "OBJECT", "TRANSFORM",
        "POSITION", "ROTATION", "SCALE", "TRIGGER", "COLLIDER", "PHYSICS", "RIGIDBODY",
        "PLAYER", "ENEMY", "LEVEL", "MISSION", "QUEST", "HEALTH", "SCORE", "POWERUP",
        "ITEM", "WEAPON", "DAMAGE", "SPAWN", "UNITY", "BUILD"
    };

    void Start()
    {
        endGameText.text = "";
        GenerateNewWord();
    }

    void Update()
    {
        if (gameOver) return;

        foreach (char c in Input.inputString)
        {
            HandleInput(c);
        }
    }

    void HandleInput(char c)
    {
        if (c == '\b') // Backspace
        {
            if (typedWord.Length > 0)
                typedWord = typedWord.Substring(0, typedWord.Length - 1);
        }
        else if (char.IsLetter(c))
        {
            c = useUppercase ? char.ToUpper(c) : char.ToLower(c);
            typedWord += c;

            if (typedWord == currentWord)
            {
                score++;
                if (score >= maxScore)
                {
                    EndGame();
                    return;
                }

                GenerateNewWord();
            }
        }

        currentInputText.text = typedWord;
        scoreText.text = $"Score: {score}/{maxScore}";
    }

    void GenerateNewWord()
    {
        if (usedWords.Count >= gameWords.Count)
        {
            EndGame();
            return;
        }

        string newWord;
        int safetyCounter = 0;

        do
        {
            newWord = gameWords[Random.Range(0, gameWords.Count)];
            safetyCounter++;
        }
        while (usedWords.Contains(newWord) && safetyCounter < 200);

        usedWords.Add(newWord);
        currentWord = newWord;
        typedWord = "";

        wordToTypeText.text = currentWord;
        currentInputText.text = "";

        // Gọi hiệu ứng fade-in từ dưới lên mỗi lần sinh từ
        StartCoroutine(FadeInWord(wordToTypeText, 0.5f, 50f));
    }

    IEnumerator FadeInWord(TextMeshProUGUI text, float duration, float moveDistance)
    {
        CanvasGroup cg = text.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = text.gameObject.AddComponent<CanvasGroup>();

        cg.alpha = 0;

        RectTransform rt = text.rectTransform;
        Vector2 startPos = rt.anchoredPosition - new Vector2(0, moveDistance);
        Vector2 endPos = rt.anchoredPosition;

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            cg.alpha = Mathf.Lerp(0, 1, t);
            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, t);

            yield return null;
        }

        cg.alpha = 1;
        rt.anchoredPosition = endPos;
    }

    void EndGame()
    {
        gameOver = true;
        wordToTypeText.text = "";
        currentInputText.text = "";
        scoreText.text = $"Final Score: {score}/{maxScore}";
        endGameText.text = " Bạn đã hoàn thành! ";

        Debug.Log("Game Over — Đạt đủ điểm hoặc hết từ!");
    }
}
