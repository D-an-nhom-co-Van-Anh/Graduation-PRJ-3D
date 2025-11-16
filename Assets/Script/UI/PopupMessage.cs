using UnityEngine;
using TMPro;
using DG.Tweening;

public class PopupMessage : UICanvas
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private CanvasGroup canvasGroup;

    private float showDuration = 2f;

    public override void Open()
    {
        base.Open();
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// Hi?n th? thông báo r?i t? trên xu?ng r?i fade out trong duration giây
    /// </summary>
    public void Show(string message, float duration)
    {
        showDuration = duration;
        messageText.text = message;
        DOTween.Kill(canvasGroup);
        DOTween.Kill(messageText.rectTransform);

        canvasGroup.alpha = 0;
        RectTransform rect = messageText.rectTransform;

        Vector2 startPos = rect.anchoredPosition + new Vector2(0, 150f); 
        Vector2 endPos = rect.anchoredPosition;

        rect.anchoredPosition = startPos;

        Sequence seq = DOTween.Sequence();

        seq.Append(canvasGroup.DOFade(1, 0.3f)) 
           .Join(rect.DOAnchorPos(endPos, 0.5f).SetEase(Ease.OutBack)) 
           .AppendInterval(showDuration - 0.5f) 
           .Append(canvasGroup.DOFade(0, 0.5f)) 
           .Join(rect.DOAnchorPos(endPos - new Vector2(0, 60f), 0.5f).SetEase(Ease.InCubic)) 
           .OnComplete(() => CloseDirect());
    }
}
