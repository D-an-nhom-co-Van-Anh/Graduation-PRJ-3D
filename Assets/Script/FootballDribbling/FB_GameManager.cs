using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;
public class FB_GameManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI messageText;

    public float totalTime = 30f;
    private float currentTime;
    private int score;
    private bool isGameOver = false;

    private void Start()
    {
        Cursor.visible = false;
        currentTime = totalTime;
        messageText.text = "";
    }

    private void Update()
    {
        if (isGameOver) return;

        currentTime -= Time.deltaTime;
        if (currentTime <= 0)
        {
            GameOver(false);
        }

        timerText.text = $"{Mathf.Ceil(currentTime)}";
        scoreText.text = $"{score}";
    }

    public void OnHitObstacle()
    {
        currentTime -= 2f;
        score -= 10;
        messageText.text = "Chạm cọc!";
        StopCoroutine(nameof(ClearMsg));
        StartCoroutine(ClearMsg());
        GameOver(false);
    }

    public void OnReachGoal()
    {
        GameOver(true);
    }

    private void GameOver(bool success)
    {
        isGameOver = true;
        if (success)
        {
            messageText.text = "Hoàn thành! Bạn nhận được CUP!";
        }
        else
        {
            messageText.text = "Bạn thua";
        }
    }

    private IEnumerator ClearMsg()
    {
        yield return new WaitForSeconds(1.5f);
        messageText.text = "";
    }
}
