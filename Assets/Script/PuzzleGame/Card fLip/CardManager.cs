using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    // Singleton — để có thể truy cập CardManager dễ dàng từ mọi nơi
    public static CardManager Instance;

    // Danh sách lưu lại các thẻ đang được lật
    private List<CardFlipEffect> flippedCards = new List<CardFlipEffect>();

    private void Awake()
    {
        // Khởi tạo singleton
        Instance = this;
    }

    /// <summary>
    /// Hàm được gọi khi một lá bài được lật lên.
    /// </summary>
    public void RegisterFlip(CardFlipEffect card)
    {
        // Nếu đã có 2 lá đang được lật thì không cho thêm nữa
        if (flippedCards.Count >= 2) return;

        // Thêm lá bài vừa lật vào danh sách
        flippedCards.Add(card);

        // Khi có đủ 2 lá trong danh sách thì bắt đầu kiểm tra xem có trùng không
        if (flippedCards.Count == 2)
        {
            StartCoroutine(CheckMatch());
        }
    }

    /// <summary>
    /// Coroutine kiểm tra xem hai lá bài đang mở có trùng nhau không.
    /// Nếu trùng → ẩn (thu hồi), nếu khác → lật lại.
    /// </summary>
    private IEnumerator CheckMatch()
    {
        // Chờ một chút để người chơi thấy hai lá bài đã lật xong
        yield return new WaitForSeconds(1f);

        // So sánh ID của hai lá bài
        if (flippedCards[0].cardID == flippedCards[1].cardID)
        {
            // ✅ Giống nhau → gọi hàm ẩn bài
            flippedCards[0].HideCard();
            flippedCards[1].HideCard();
        }
        else
        {
            // ❌ Khác nhau → gọi hàm lật ngược lại
            flippedCards[0].FlipBack();
            flippedCards[1].FlipBack();
        }

        // Dọn danh sách để chuẩn bị cho lần lật tiếp theo
        flippedCards.Clear();
    }
}
