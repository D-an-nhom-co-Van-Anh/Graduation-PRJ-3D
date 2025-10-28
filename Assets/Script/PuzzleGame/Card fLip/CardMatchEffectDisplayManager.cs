using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[System.Serializable]
public class MatchDisplay
{
    [Header("ID của cặp thẻ trùng")]
    public int cardID;

    [Header("Ảnh effect trên màn chơi (UI Image hoặc SpriteRenderer)")]
    public GameObject effectObject;

    [Header("Thời gian hiển thị (giây)")]
    public float displayDuration = 2f;

    [Header("Âm thanh khi xuất hiện (tùy chọn)")]
    public AudioClip matchSound;
}

public class CardMatchEffectDisplayManager : MonoBehaviour
{
    public static CardMatchEffectDisplayManager Instance;

    [Header("Danh sách effect ứng với từng Card ID")]
    [SerializeField] private List<MatchDisplay> matchDisplays = new List<MatchDisplay>();

    [Header("Audio Source phát âm thanh (có thể bỏ trống nếu không dùng)")]
    [SerializeField] private AudioSource audioSource;

    [Header("Hiệu ứng xuất hiện")]
    [SerializeField] private float appearDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.6f;
    [SerializeField] private float zoomScale = 1.2f;
    [SerializeField] private bool shakeCamera = true;

    private void Awake()
    {
        Instance = this;

        // Ẩn toàn bộ effect khi bắt đầu
        foreach (var display in matchDisplays)
        {
            if (display.effectObject != null)
            {
                display.effectObject.SetActive(false);
                var cg = display.effectObject.GetComponent<CanvasGroup>();
                if (cg == null)
                    cg = display.effectObject.AddComponent<CanvasGroup>();
                cg.alpha = 0f;
            }
        }
    }

    /// <summary>
    /// Hiển thị effect tương ứng khi match, có animation DOTween, rồi tự biến mất.
    /// </summary>
    public void ShowEffect(int cardID)
    {
        MatchDisplay display = matchDisplays.Find(d => d.cardID == cardID);
        if (display == null)
        {
            Debug.LogWarning($"⚠️ Không tìm thấy effect cho card ID {cardID}");
            return;
        }

        var obj = display.effectObject;
        if (obj == null)
        {
            Debug.LogWarning($"⚠️ Effect object cho card ID {cardID} bị null!");
            return;
        }

        obj.SetActive(true);
        obj.transform.localScale = Vector3.one * 0.8f;
        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;

        // 🔊 Phát âm thanh nếu có
        if (audioSource != null && display.matchSound != null)
            audioSource.PlayOneShot(display.matchSound);

        // 🎬 Tạo animation bằng DOTween
        Sequence seq = DOTween.Sequence();

        // Fade in + zoom
        seq.Append(cg.DOFade(1f, appearDuration).SetEase(Ease.OutQuad));
        seq.Join(obj.transform.DOScale(zoomScale, appearDuration).SetEase(Ease.OutBack));

        // Giữ trên màn hình
        seq.AppendInterval(display.displayDuration);

        // Fade out + thu nhỏ lại
        seq.Append(cg.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad));
        seq.Join(obj.transform.DOScale(0.6f, fadeOutDuration).SetEase(Ease.InBack));

        // Khi hoàn tất → tắt object
        seq.OnComplete(() =>
        {
            obj.SetActive(false);
            cg.alpha = 0f;
            obj.transform.localScale = Vector3.one;
        });

        // 👀 Rung camera nhẹ khi xuất hiện
        if (shakeCamera && Camera.main != null)
        {
            Camera.main.DOShakePosition(
                0.3f,
                8f,
                15,
                90f,
                true,
                ShakeRandomnessMode.Full
            );
        }
    }

    /// <summary>
    /// Reset toàn bộ effect (ví dụ khi restart game).
    /// </summary>
    public void ResetAllEffects()
    {
        foreach (var display in matchDisplays)
        {
            if (display.effectObject != null)
            {
                display.effectObject.SetActive(false);
                var cg = display.effectObject.GetComponent<CanvasGroup>();
                if (cg != null) cg.alpha = 0f;
                display.effectObject.transform.localScale = Vector3.one;
            }
        }
    }
}
