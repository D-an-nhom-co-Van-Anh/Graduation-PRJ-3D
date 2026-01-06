using System.Collections.Generic;
using UnityEngine;

public class CardShuffler : MonoBehaviour
{
    [Header("Parent chứa tất cả các lá bài UI")]
    [SerializeField] private Transform cardParent;

    [Header("Tự động trộn khi bắt đầu game")]
    [SerializeField] private bool shuffleOnStart = true;

    private List<Transform> cardList = new List<Transform>();

    private void Start()
    {
        if (shuffleOnStart)
        {
            ShuffleCards();
        }
    }

    public void ShuffleCards()
    {
        if (cardParent == null)
        {
            Debug.LogWarning("⚠️ Chưa gán Card Parent cho CardShuffler!");
            return;
        }

        // Lấy danh sách tất cả các card con
        cardList.Clear();
        foreach (Transform card in cardParent)
        {
            cardList.Add(card);
        }

        // Lưu lại vị trí ban đầu của tất cả card
        List<Vector3> originalPositions = new List<Vector3>();
        foreach (Transform card in cardList)
        {
            originalPositions.Add(card.localPosition);
        }

        // Fisher-Yates shuffle: trộn ngẫu nhiên danh sách vị trí
        for (int i = 0; i < originalPositions.Count; i++)
        {
            int randomIndex = Random.Range(i, originalPositions.Count);
            (originalPositions[i], originalPositions[randomIndex]) =
                (originalPositions[randomIndex], originalPositions[i]);
        }

        // Gán lại vị trí mới theo thứ tự đã trộn
        for (int i = 0; i < cardList.Count; i++)
        {
            cardList[i].localPosition = originalPositions[i];
        }

        Debug.Log(" Đã trộn vị trí ngẫu nhiên các lá bài!");
    }
}
