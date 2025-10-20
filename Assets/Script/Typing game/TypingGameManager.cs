using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TypingGameManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text hiển thị từ cần gõ")]
    public TMP_Text wordDisplay;

    [Tooltip("Script hiệu ứng chữ bay")]
    public TypingEffects typingEffect;

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

    void Start()
    {
        PickNewWord();
    }

    void Update()
    {
        foreach (char c in Input.inputString)
        {
            HandleInput(c);
        }
    }

    void HandleInput(char inputChar)
    {
        // Bỏ qua nếu không có từ hiện tại
        if (string.IsNullOrEmpty(currentWord)) return;

        // Chuyển ký tự thành chữ hoa để so sánh dễ hơn
        inputChar = char.ToUpper(inputChar);

        // ✅ Nếu gõ đúng ký tự
        if (inputChar == currentWord[currentIndex])
        {
            // Gọi hiệu ứng chữ bay ra từ vị trí chữ đang hiển thị
            Vector3 charWorldPos = GetCharacterWorldPosition(currentIndex);
            typingEffect.SpawnFlyingLetter(inputChar, charWorldPos);

            currentIndex++;

            // Nếu hoàn thành từ
            if (currentIndex >= currentWord.Length)
            {
                typingEffect.PlayWordCompleteEffect();
                Invoke(nameof(PickNewWord), 0.5f); // đợi nửa giây rồi đổi từ mới
            }
            else
            {
                UpdateWordDisplay();
            }
        }
        else
        {
            // ❌ Nếu gõ sai ký tự
            typingEffect.PlayWrongLetterEffect(wordDisplay.transform);
        }
    }

    void PickNewWord()
    {
        currentWord = wordList[Random.Range(0, wordList.Count)];
        currentIndex = 0;
        UpdateWordDisplay();
    }

    void UpdateWordDisplay()
    {
        if (currentWord == null) return;

        // Phần đã gõ: màu xanh
        string typedPart = $"<color=#00FF7F>{currentWord.Substring(0, currentIndex)}</color>";

        // Phần còn lại: màu trắng
        string remainingPart = currentWord.Substring(currentIndex);

        wordDisplay.text = typedPart + remainingPart;
    }

    
    Vector3 GetCharacterWorldPosition(int index)
    {
        if (wordDisplay.textInfo == null || index < 0 || index >= wordDisplay.textInfo.characterCount)
            return wordDisplay.transform.position;

        var charInfo = wordDisplay.textInfo.characterInfo[index];

        if (!charInfo.isVisible)
            return wordDisplay.transform.position;

        // Tính vị trí trung tâm ký tự
        Vector3 midPoint = (charInfo.topLeft + charInfo.bottomRight) / 2f;
        Vector3 worldPos = wordDisplay.transform.TransformPoint(midPoint);
        return worldPos;
    }
}
