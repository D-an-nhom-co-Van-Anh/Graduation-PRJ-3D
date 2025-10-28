using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MatchEffectData
{
    [Header("Card ID tương ứng với cặp thẻ")]
    public int cardID;

    [Header("Ảnh hiệu ứng khi match")]
    public Sprite effectSprite;

    [Header("Âm thanh khi match (tùy chọn)")]
    public AudioClip matchSound;
}

public class CardMatchEffectManager : MonoBehaviour
{
    public static CardMatchEffectManager Instance;

    [Header("UI hiển thị hiệu ứng")]
    [SerializeField] private Image effectImage;

    [Header("Nguồn phát âm thanh")]
    [SerializeField] private AudioSource audioSource;

    [Header("Danh sách hiệu ứng theo từng Card ID")]
    [SerializeField] private List<MatchEffectData> matchEffects = new List<MatchEffectData>();

    [Header("Cài đặt animation DOTween")]
    [SerializeField] private float appearDuration = 0.3f;
    [SerializeField] private float holdDuration = 1.0f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float zoomScale = 1.4f;
    [SerializeField] private bool shakeOnAppear = true;

    private bool isPlaying = false;

    private void Awake()
    {
        Instance = this;

        // 🔧 Đảm bảo effect bị ẩn hoàn toàn khi bắt đầu scene
        if (effectImage != null)
        {
            effectImage.gameObject.SetActive(false);
            effectImage.color = new Color(1, 1, 1, 0);
            effectImage.transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// Gọi khi hai lá bài trùng khớp để hiển thị hiệu ứng.
    /// </summary>
    public void PlayEffect(int cardID)
    {
        if (isPlaying || effectImage == null) return;

        // Tìm dữ liệu hiệu ứng phù hợp theo cardID
        MatchEffectData data = matchEffects.Find(e => e.cardID == cardID);
        if (data == null)
        {
            Debug.LogWarning($"⚠️ Không tìm thấy hiệu ứng cho Card ID: {cardID}");
            return;
        }

        StartCoroutine(PlayEffectRoutine(data));
    }

    private IEnumerator PlayEffectRoutine(MatchEffectData data)
    {
        isPlaying = true;

        // Cập nhật sprite và reset trạng thái
        effectImage.sprite = data.effectSprite;
        effectImage.gameObject.SetActive(true);
        effectImage.color = new Color(1, 1, 1, 0);
        effectImage.transform.localScale = Vector3.one * 0.5f;

        // 🔊 Phát âm thanh nếu có
        if (audioSource != null && data.matchSound != null)
            audioSource.PlayOneShot(data.matchSound);

        // 🎬 Tạo chuỗi animation DOTween
        Sequence seq = DOTween.Sequence();

        // Fade + zoom in
        seq.Append(effectImage.DOFade(1f, appearDuration).SetEase(Ease.OutQuad));
        seq.Join(effectImage.transform.DOScale(zoomScale, appearDuration).SetEase(Ease.OutBack));

        // Giữ trong vài giây
        seq.AppendInterval(holdDuration);

        // Fade + thu nhỏ dần
        seq.Append(effectImage.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad));
        seq.Join(effectImage.transform.DOScale(0.3f, fadeOutDuration).SetEase(Ease.InBack));

        // Khi xong → ẩn đi
        seq.OnComplete(() =>
        {
            effectImage.gameObject.SetActive(false);
            effectImage.transform.localScale = Vector3.one;
            isPlaying = false;
        });

        // Rung camera nếu bật
        if (shakeOnAppear && Camera.main != null)
        {
            Camera.main.DOShakePosition(
                0.3f,                   // thời gian rung
                10f,                    // biên độ rung
                20,                     // số lần rung
                90f,                    // độ ngẫu nhiên
                true,                   // fade out
                ShakeRandomnessMode.Full // kiểu rung ngẫu nhiên
            );
        }

        yield return seq.WaitForCompletion();
    }

    /// <summary>
    /// Reset effect về trạng thái ẩn (dùng khi restart game hoặc load lại scene).
    /// </summary>
    public void ResetEffect()
    {
        if (effectImage != null)
        {
            effectImage.DOKill();
            effectImage.gameObject.SetActive(false);
            effectImage.color = new Color(1, 1, 1, 0);
            effectImage.transform.localScale = Vector3.one;
        }

        isPlaying = false;
    }
}
