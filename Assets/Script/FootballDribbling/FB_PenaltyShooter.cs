using UnityEngine;

public class FB_PenaltyShooter : MonoBehaviour
{
    public Rigidbody ball;
    public Transform ballSpawn;
    [Header("Shoot Settings")]
    public float minPower = 14f;
    public float maxPower = 24f;
    public float minLift = 4f;
    public float maxLift = 7f;
    public float aimNoise = 0.2f; // độ lệch khi sút (giống FIFA)
    public bool canStart = false;
    public Transform goalPlane;  // Plane dùng để chọn điểm ngắm
    private Vector3 targetPoint;
    [SerializeField] private FB_PlayerController playerController;
    private void Awake()
    {
        canStart = false;
    }
    public void StartPenalty()
    {
        canStart = true;
    }


    void Update()
    {
        if (!canStart) return;

        // Dùng raycast chuột vào mặt phẳng khung thành
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(-goalPlane.forward, goalPlane.position);

        if (plane.Raycast(ray, out float enter))
        {
            targetPoint = ray.GetPoint(enter);
            // Vẽ đường debug để thấy nơi đang ngắm
            Debug.DrawLine(ray.origin, targetPoint, Color.red);
        }

        // Khi click chuột trái -> Sút
        if (Input.GetMouseButtonDown(0))
        {
            ShootBall();
        }
    }

    void ShootBall()
    {
        Debug.Log("Shoot Penalty");
        ball.transform.position = ballSpawn.position;
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;

        // Tính hướng
        Vector3 dir = (targetPoint - ballSpawn.position).normalized;

        // Tạo chút lệch ngẫu nhiên (giống FIFA)
        dir.x += Random.Range(-aimNoise, aimNoise);
        dir.y += Random.Range(-aimNoise * 0.5f, aimNoise * 0.5f);
        dir.Normalize();

        // Tính lực
        float distance = Vector3.Distance(ballSpawn.position, targetPoint);
        float power = Mathf.Clamp(distance * 2.0f, minPower, maxPower);
        float lift = Mathf.Lerp(minLift, maxLift, distance / 20f);

        // Tạo lực tổng (thêm Vector3.up để có quỹ đạo cong)
        Vector3 shootForce = (dir * power) + (Vector3.up * lift);
        playerController.AimAndShootPenalty(shootForce);
        // Gọi animation hoặc AI thủ môn
       // playerController?.OnShoot(targetPoint, shootForce);
    }
}
