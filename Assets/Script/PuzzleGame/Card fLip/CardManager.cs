using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    // Danh sách các lá đang được lật
    private List<CardFlipEffect> flippedCards = new List<CardFlipEffect>();

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Ghi nhận một lá bài vừa được lật.
    /// </summary>
    public void RegisterFlip(CardFlipEffect card)
    {
        // Nếu đã có 2 lá đang mở, không nhận thêm click
        if (flippedCards.Count >= 2) return;

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
        // Cho người chơi thấy trong 1 giây trước khi xử lý
        yield return new WaitForSeconds(1f);

        var first = flippedCards[0];
        var second = flippedCards[1];

        if (first.cardID == second.cardID)
        {
            // ✅ Giống nhau
            first.HideCard();
            second.HideCard();
        }
        else
        {
            // ❌ Không trùng → lật ngược lại
            first.FlipBack();
            second.FlipBack();
        }

        // Làm sạch danh sách
        flippedCards.Clear();
    }
}
