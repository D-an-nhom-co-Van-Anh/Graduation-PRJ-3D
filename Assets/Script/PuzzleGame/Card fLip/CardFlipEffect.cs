using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CardFlipEffect : MonoBehaviour, IPointerClickHandler
{
    public int cardID; // để phân biệt loại thẻ (giống nhau nếu ID giống)
    private bool flipped = false;
    private bool matched = false;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (matched || flipped) return; // Không lật nếu đã khớp hoặc đang mở

        Flip();
        CardManager.Instance.RegisterFlip(this);
    }

    public void Flip()
    {
        flipped = true;
        transform.DORotate(new Vector3(0, 180f, 0), 0.25f)
            .SetEase(Ease.InOutBack);
    }

    public void FlipBack()
    {
        flipped = false;
        transform.DORotate(new Vector3(0, 0f, 0), 0.25f)
            .SetEase(Ease.InOutBack);
    }

    public void HideCard()
    {
        matched = true;
        transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack);
    }
}
