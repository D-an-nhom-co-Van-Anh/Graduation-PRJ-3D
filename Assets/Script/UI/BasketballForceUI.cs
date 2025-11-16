using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BasketballForceUI : UICanvas
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image forceFillImage;

    [Header("Settings")]
    public float fadeDuration = 0.25f;

    private Tween fadeTween;

    public override void Open()
    {
        base.Open();
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// G?i khi ng??i ch?i b?t ??u gi? phím F
    /// </summary>
    public void ShowForceUI()
    {
        

        DOTween.Kill(fadeTween);
        canvasGroup.alpha = 0;
        fadeTween = canvasGroup.DOFade(1, fadeDuration);

        forceFillImage.fillAmount = 0f;
    }

    /// <summary>
    /// Truy?n vào giá tr? 0 ? 1 ?? th? hi?n l?c ném
    /// </summary>
    public void UpdateForce(float percent)
    {
        forceFillImage.fillAmount = Mathf.Clamp01(percent);
    }

   
}
