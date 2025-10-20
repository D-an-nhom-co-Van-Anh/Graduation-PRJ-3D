using UnityEngine;
using TMPro;
using DG.Tweening;

public class TypingEffect : MonoBehaviour
{
    public TextMeshProUGUI textPrefab; // Prefab cho từng ký tự bay
    public float fadeDuration = 0.4f;
    public float moveDistance = 80f;
    public Ease moveEase = Ease.OutBack;

    public void PlayWordEffect(string word)
    {
        // Vị trí trung tâm để tạo ký tự
        Vector3 startPos = transform.position;
        float spacing = 30f; // khoảng cách giữa các chữ cái

        for (int i = 0; i < word.Length; i++)
        {
            char c = word[i];
            TextMeshProUGUI letter = Instantiate(textPrefab, transform.parent);
            letter.text = c.ToString();
            letter.fontSize = textPrefab.fontSize;
            letter.color = textPrefab.color;

            // Tính vị trí chữ cái dựa vào index
            letter.rectTransform.anchoredPosition = new Vector2(startPos.x + i * spacing, startPos.y);

            // Hiệu ứng bay
            Vector3 targetPos = letter.rectTransform.anchoredPosition + new Vector2(0, moveDistance);
            letter.DOFade(0, fadeDuration).SetEase(Ease.InExpo);
            letter.rectTransform.DOAnchorPos(targetPos, fadeDuration).SetEase(moveEase)
                .OnComplete(() => Destroy(letter.gameObject));
        }
    }
}
