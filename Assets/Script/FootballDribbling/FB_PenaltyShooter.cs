using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FB_PenaltyShooter : MonoBehaviour
{
    public Rigidbody ball;
    public Transform ballSpawn;
    [Header("Shoot Settings")]
    public float minPower = 5f;
    public float maxPower = 10f;
    public float minLift = 3f;
    public float maxLift = 6f;
    public float aimNoise = 0.2f; // độ lệch khi sút (giống FIFA)
    public bool canStart = false;
    private float curruntPower;
    public Transform goalPlane;  // Plane dùng để chọn điểm ngắm
    private Vector3 targetPoint;
    public float distanceFromCamera = 14.27f;
    [SerializeField] private FB_PlayerController playerController;
    [SerializeField] private Slider slider;
    [SerializeField] private FB_LineRenderer lineRenderer;
    private void Awake()
    {
        canStart = false;
        curruntPower = minPower;
        slider.value = 0;
    }
    public void StartPenalty()
    {
        canStart = true;
    }


    void Update()
    {
        if (!canStart) return;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distanceFromCamera;

        // Chuyển sang world position
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // Gán vị trí cho object
        targetPoint = worldPos;
        // Khi click chuột trái -> Sút
        if (Input.GetMouseButton(0))
        {
            curruntPower += Time.deltaTime*20f;
            if (curruntPower <= maxPower)
            {
                slider.value = (float)(curruntPower-minPower) / (maxPower-minPower);
            }
            curruntPower = Mathf.Clamp(curruntPower, minPower, maxPower);
        }
        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(ShootBall());
            canStart = false;
        }
    }

    IEnumerator ShootBall()
    {
        Debug.Log("Shoot Penalty");
        ball.transform.position = ballSpawn.position;
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;

        // Tính hướng
        Vector3 dir = (targetPoint - ballSpawn.position).normalized;

        // Tạo chút lệch ngẫu nhiên (giống FIFA)
       // dir.x += Random.Range(-aimNoise, aimNoise);
       // dir.y += Random.Range(-aimNoise * 0.5f, aimNoise * 0.5f);
        dir.Normalize();

        // Tính lực
        float distance = Vector3.Distance(ballSpawn.position, targetPoint);
        float power = curruntPower;
            //Mathf.Clamp(distance * 2.0f, minPower, maxPower);
        float lift = Mathf.Lerp(minLift, maxLift, distance / 20f);

        // Tạo lực tổng (thêm Vector3.up để có quỹ đạo cong)
        Vector3 shootForce = (dir * power) + (Vector3.up * lift);
        lineRenderer.DrawTrajectory(ballSpawn.position, targetPoint);
        yield return new WaitForSeconds(0.5f);
        playerController.AimAndShootPenalty(shootForce);
        lineRenderer.Clear();
        AudioManager.Instance.PlayMusic("soccerBallKick");
        // Gọi animation hoặc AI thủ môn
       // playerController?.OnShoot(targetPoint, shootForce);
    }

}
