using UnityEngine;

public class FB_GoalKeeperMove : MonoBehaviour
{
    public Transform leftDivePoint;     // điểm bay sang trái
    public Transform rightDivePoint;    // điểm bay sang phải
    public Transform centerPoint;       // vị trí đứng giữa
    public float diveSpeed = 10f;
    public float reactDelay = 0.3f;     // độ trễ phản xạ
    bool isDiving = false;
    [SerializeField] private FB_GameManager manager;
    Vector3 targetDivePos;
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
       // ReturnToCenter();
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
            ballRb.linearVelocity = -ballRb.linearVelocity * 2.5f;
            manager.GameOver(false);
            Debug.Log("Goalkeeper saved the shot!");
        }
    }
    public void StartMove(Vector3 targetDivePosition)
    {
        this.targetDivePos = targetDivePosition;
        StartCoroutine(DiveAfterDelay());
    }
}
