using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

[System.Serializable]
public class MatchDisplay
{
    [Header("ID của cặp thẻ trùng")]
    public int cardID;

    [Header("GameObject chứa hình effect (UI Image hoặc SpriteRenderer)")]
    public GameObject effectObject;

    [Header("Âm thanh khi xuất hiện (AudioClip riêng cho effect)")]
    public AudioClip matchSound;

    [Header("Thời gian hiển thị (giây)")]
    public float displayDuration = 2f;
}

public class CardMatchEffectDisplayManager : MonoBehaviour
{
    public static CardMatchEffectDisplayManager Instance;

    [Header("Danh sách effect ứng với từng Card ID")]
    [SerializeField] private List<MatchDisplay> matchDisplays = new List<MatchDisplay>();

    [Header("Nguồn phát âm thanh chung (sẽ dùng để PlayOneShot clip)")]
    [SerializeField] private AudioSource globalAudioSource;

    [Header("Hiệu ứng DOTween")]
    [SerializeField] private float appearDuration = 0.4f;
    [SerializeField] private float fadeOutDuration = 0.6f;
    [SerializeField] private float zoomScale = 1.2f;
    [SerializeField] private bool shakeCamera = true;

    private void Awake()
    {
        Instance = this;

        // Nếu chưa có AudioSource, tự thêm
        if (globalAudioSource == null)
        {
            globalAudioSource = gameObject.AddComponent<AudioSource>();
            globalAudioSource.playOnAwake = false;
        }

        // Ẩn tất cả effect khi bắt đầu
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

    /// Gọi khi match đúng 2 thẻ, sẽ hiển thị effect + phát âm thanh tương ứng.
    public void ShowEffect(int cardID)
    {
        MatchDisplay display = matchDisplays.Find(d => d.cardID == cardID);
        if (display == null)
        {
            Debug.LogWarning($"Không tìm thấy effect cho card ID {cardID}");
            return;
        }

        var obj = display.effectObject;
        if (obj == null)
        {
            Debug.LogWarning($" Effect object cho card ID {cardID} bị null!");
            return;
        }

        var cg = obj.GetComponent<CanvasGroup>();
        if (cg == null) cg = obj.AddComponent<CanvasGroup>();

        obj.SetActive(true);
        obj.transform.localScale = Vector3.one * 0.8f;
        cg.alpha = 0f;

        // Phát âm thanh nếu có
        if (display.matchSound != null && globalAudioSource != null)
            globalAudioSource.PlayOneShot(display.matchSound);

        // Animation DOTween: fade in → giữ → fade out
        Sequence seq = DOTween.Sequence();

        seq.Append(cg.DOFade(1f, appearDuration).SetEase(Ease.OutQuad));
        seq.Join(obj.transform.DOScale(zoomScale, appearDuration).SetEase(Ease.OutBack));

        seq.AppendInterval(display.displayDuration);

        seq.Append(cg.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad));
        seq.Join(obj.transform.DOScale(0.7f, fadeOutDuration).SetEase(Ease.InBack));

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
    /// Reset tất cả effect về trạng thái ẩn (khi restart game).
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
