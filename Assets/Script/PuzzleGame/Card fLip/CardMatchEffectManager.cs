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

    [Header("Nguồn phát âm thanh hiệu ứng")]
    [SerializeField] private AudioSource audioSource;

    [Header("Nguồn phát nhạc nền (Background Music)")]
    [SerializeField] private AudioSource bgmSource; // 🔥 thêm dòng này

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

        if (effectImage != null)
        {
            effectImage.gameObject.SetActive(false);
            effectImage.color = new Color(1, 1, 1, 0);
            effectImage.transform.localScale = Vector3.one;
        }
    }

    public void PlayEffect(int cardID)
    {
        if (isPlaying || effectImage == null) return;

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

        // Dừng nhạc nền nếu đang phát 🔇
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Pause();

        effectImage.sprite = data.effectSprite;
        effectImage.gameObject.SetActive(true);
        effectImage.color = new Color(1, 1, 1, 0);
        effectImage.transform.localScale = Vector3.one * 0.5f;

        float effectDuration = 0f;

        // 🔊 Phát âm thanh hiệu ứng nếu có
        if (audioSource != null && data.matchSound != null)
        {
            audioSource.PlayOneShot(data.matchSound);
            effectDuration = data.matchSound.length; // lưu lại độ dài clip
        }

        // 🎬 Animation
        Sequence seq = DOTween.Sequence();
        seq.Append(effectImage.DOFade(1f, appearDuration).SetEase(Ease.OutQuad));
        seq.Join(effectImage.transform.DOScale(zoomScale, appearDuration).SetEase(Ease.OutBack));
        seq.AppendInterval(holdDuration);
        seq.Append(effectImage.DOFade(0f, fadeOutDuration).SetEase(Ease.InQuad));
        seq.Join(effectImage.transform.DOScale(0.3f, fadeOutDuration).SetEase(Ease.InBack));

        seq.OnComplete(() =>
        {
            effectImage.gameObject.SetActive(false);
            effectImage.transform.localScale = Vector3.one;
            isPlaying = false;

            // 🔊 Tiếp tục phát nhạc nền sau khi hiệu ứng xong
            if (bgmSource != null)
                bgmSource.UnPause();
        });

        if (shakeOnAppear && Camera.main != null)
        {
            Camera.main.DOShakePosition(0.3f, 10f, 20, 90f, true, ShakeRandomnessMode.Full);
        }

        yield return seq.WaitForCompletion();
    }

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
