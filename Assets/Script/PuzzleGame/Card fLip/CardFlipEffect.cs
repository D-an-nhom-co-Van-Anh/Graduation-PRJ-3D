using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardFlipEffect : MonoBehaviour, IPointerClickHandler
{
    [Header("Card ID để so khớp")]
    public int cardID;

    private bool flipped = false;
    private bool matched = false;

    [SerializeField] private Image frontImage;
    [SerializeField] private Image backImage;

    /// <summary>
    /// Xử lý khi người chơi click vào lá bài.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // Nếu đã khớp hoặc đang lật thì bỏ qua
        if (matched || flipped) return;

        // 🔒 Hỏi CardManager xem có cho phép lật không
        if (!CardManager.Instance.CanFlipCard())
            return;

        Debug.Log($"Clicked on card: {name}");
        Flip();
        CardManager.Instance.RegisterFlip(this);
    }

    /// <summary>
    /// Lật bài lên.
    /// </summary>
    public void Flip()
    {
        flipped = true;
        transform.DOScaleX(0f, 0.15f).OnComplete(() => {
            frontImage.enabled = true;
            backImage.enabled = false;
            transform.DOScaleX(1f, 0.15f);
        });
    }

    public void FlipBack()
    {
        flipped = false;
        transform.DOScaleX(0f, 0.15f).OnComplete(() => {
            frontImage.enabled = false;
            backImage.enabled = true;
            transform.DOScaleX(1f, 0.15f);
        });
    }

    /// <summary>
    /// Ẩn bài khi trùng khớp.
    /// </summary>
    public void HideCard()
    {
        matched = true;

        // Hiệu ứng thu nhỏ biến mất
        transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => Debug.Log($"{name} matched and hidden!"));
    }
}
