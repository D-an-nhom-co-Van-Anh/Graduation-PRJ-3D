using UnityEngine;

public class FB_GoalKeeper : MonoBehaviour
{
    public Transform leftDivePoint;     // điểm bay sang trái
    public Transform rightDivePoint;    // điểm bay sang phải
    public Transform centerPoint;       // vị trí đứng giữa
    public float diveSpeed = 10f;
    public float reactDelay = 0.3f;     // độ trễ phản xạ
    public Animator animator;
    public int centerNum = 3;
    bool isDiving = false;
    [SerializeField] private FB_GameManager manager;
    Vector3 targetDivePos;

    public void ReactToShot(Vector3 ballDirection)
    {
        if (isDiving) return;
        int randomValue = Random.Range(1, 4);
        // Chọn hướng bay (ngẫu nhiên hoặc dựa trên hướng bóng)
        bool diveRight = Random.value > 0.5f;

        // Nếu muốn thông minh hơn: so sánh hướng bóng với khung thành
        if (ballDirection.x > 0.2f) diveRight = true;
        else if (ballDirection.x < -0.2f) diveRight = false;
        if (randomValue == centerNum)
        {
            targetDivePos = centerPoint.position;
        }
        else
        {
            targetDivePos = diveRight ? rightDivePoint.position : leftDivePoint.position;
        }
        // Gọi animation
        //animator.SetTrigger(diveRight ? "DiveRight" : "DiveLeft");

        // Bắt đầu coroutine dive
        StartCoroutine(DiveAfterDelay());
    }

    System.Collections.IEnumerator DiveAfterDelay()
    {
        isDiving = true;
        yield return new WaitForSeconds(reactDelay);

        float t = 0;
        Vector3 start = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * (diveSpeed / 5f);
            transform.position = Vector3.Lerp(start, targetDivePos, t);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        ReturnToCenter();
    }

    void ReturnToCenter()
    {
        isDiving = false;
        transform.position = centerPoint.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<FB_Ball>())
        {
            // Cản bóng – phản lại hoặc chặn đứng
            Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
            ballRb.linearVelocity = -ballRb.linearVelocity * 0.5f;
            manager.GameOver(false);
            Debug.Log("Goalkeeper saved the shot!");
        }
    }
}
