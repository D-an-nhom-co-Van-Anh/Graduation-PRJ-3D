using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    // Danh sách các lá đang được lật
    private List<CardFlipEffect> flippedCards = new List<CardFlipEffect>();

    // Biến khóa input trong khi đang kiểm tra
    private bool checkingMatch = false;

    [Header("Thời gian chờ trước khi kiểm tra (giây)")]
    [SerializeField] private float delayBeforeCheck = 1f;

    [Header("Thời gian chờ sau khi lật sai (giây)")]
    [SerializeField] private float delayAfterWrong = 0.3f;

    private void Awake()
    {
        AudioManager.Instance.StopMusic();
        Instance = this;
    }

    /// <summary>
    /// Kiểm tra xem có thể lật thêm lá bài mới hay không.
    /// </summary>
    public bool CanFlipCard()
    {
        return !checkingMatch && flippedCards.Count < 2;
    }

    /// <summary>
    /// Ghi nhận một lá bài vừa được lật.
    /// </summary>
    public void RegisterFlip(CardFlipEffect card)
    {
        // Nếu đang kiểm tra hoặc đã đủ 2 lá, bỏ qua
        if (checkingMatch || flippedCards.Count >= 2)
            return;

        flippedCards.Add(card);

        // Nếu đã có đủ 2 lá, kiểm tra trùng
        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    /// <summary>
    /// Kiểm tra 2 lá có trùng nhau không.
    /// </summary>
    private IEnumerator CheckMatch()
    {
        checkingMatch = true;

        // Cho người chơi thấy trong một khoảng thời gian trước khi xử lý
        yield return new WaitForSeconds(delayBeforeCheck);

        var first = flippedCards[0];
        var second = flippedCards[1];

        if (first.cardID == second.cardID)
        {
            // ✅ Giống nhau
            first.HideCard();
            second.HideCard();


            // ✅ Gọi hiển thị effect tương ứng trên màn chơi
            if (CardMatchEffectDisplayManager.Instance != null)
                CardMatchEffectDisplayManager.Instance.ShowEffect(first.cardID);

            // Gọi hiệu ứng tương ứng với cardID nếu có
            if (CardMatchEffectManager.Instance != null)
                CardMatchEffectManager.Instance.PlayEffect(first.cardID);

            flippedCards.Clear();
            checkingMatch = false;
        }
        else
        {
            // ❌ Không trùng → lật ngược lại
            first.FlipBack();
            second.FlipBack();

            // Đợi thêm chút thời gian để tránh spam click
            yield return new WaitForSeconds(delayAfterWrong);

            flippedCards.Clear();
            checkingMatch = false;
        }
    }
}
