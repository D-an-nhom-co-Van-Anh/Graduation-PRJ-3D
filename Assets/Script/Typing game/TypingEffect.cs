using UnityEngine;
using TMPro;
using DG.Tweening;

public class TypingEffects : MonoBehaviour
{
    [Header("Effect Prefabs")]
    public TMP_Text flyingLetterPrefab;
    public Transform flyingLetterParent;
    public Transform targetPoint;

    [Header("Effect Settings")]
    public float flyDuration = 1f;
    public float fadeDuration = 0.8f;
    public float rotationAmount = 45f;

    public void SpawnFlyingLetter(char c, Vector3 startWorldPos)
    {
        if (flyingLetterPrefab == null)
        {
            Debug.LogWarning("TypingEffect: Missing prefab reference!");
            return;
        }

        Vector3 localStartPos = flyingLetterParent.InverseTransformPoint(startWorldPos);

        // ✅ Dịch lùi lại 100 đơn vị theo trục X, y local của parent
        //localStartPos.x += 200f;
        localStartPos.y += 100f;

        // ✅ Thêm offset nhẹ để nhìn tự nhiên hơn (nếu muốn)
        Vector3 startOffset = new Vector3(
            Random.Range(-5f, 15f),
            Random.Range(-2f, 20f),
            0
        );

        Vector3 endOffset = new Vector3(
            Random.Range(-2f, 2f),
            Random.Range(20f, 30f),
            0
        );

        // ✅ Tạo chữ theo local position (dưới parent)
        TMP_Text letter = Instantiate(
            flyingLetterPrefab,
            flyingLetterParent
        );

        letter.text = c.ToString();
        letter.rectTransform.anchoredPosition = localStartPos + startOffset;

        // ✅ Di chuyển tương đối trong cùng local space
        letter.rectTransform
            .DOLocalMove(localStartPos + endOffset, flyDuration)
            .SetEase(Ease.OutExpo);

        letter.DOFade(0, fadeDuration).SetDelay(0.4f);

        letter.rectTransform
            .DORotate(
                new Vector3(0, 0, Random.Range(-rotationAmount, rotationAmount)),
                flyDuration,
                RotateMode.Fast
            );

        letter.rectTransform
            .DOScale(1.2f, 0.2f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad);

        Destroy(letter.gameObject, flyDuration + 0.8f);
    }



    public void PlayWrongLetterEffect(Transform target)
    {
        if (target == null) return;
        target.DOShakePosition(0.3f, 5f, 10, 90f);
    }

    public void PlayWordCompleteEffect()
    {
        // Có thể thêm particle hoặc âm thanh ở đây
        Debug.Log("✅ Word completed!");
    }
}
