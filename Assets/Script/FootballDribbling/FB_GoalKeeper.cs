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
    [SerializeField] private FB_GoalKeeperMove goalKeeperMove;
    Vector3 targetDivePos;

    public void ReactToShot(Vector3 ballDirection)
    {
        if (isDiving) return;
        int randomValue = Random.Range(1, 4);
        // Chọn hướng bay (ngẫu nhiên hoặc dựa trên hướng bóng)
        bool diveRight = Random.value > 0.5f;
        if (ballDirection.x > 0.2f) diveRight = true;
        else if (ballDirection.x < -0.2f) diveRight = false;
        if (randomValue == centerNum)
        {
            targetDivePos = centerPoint.position;
            // Gọi animation
            animator.SetTrigger("center");
            Debug.Log("CENTER");

        }
        else
        {
            targetDivePos = diveRight ? leftDivePoint.position:rightDivePoint.position;
            // Gọi animation
            animator.SetTrigger(diveRight ? "right": "left");
            Debug.Log("right "+diveRight);
        }
       // goalKeeperMove.StartMove(targetDivePos);
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
}
