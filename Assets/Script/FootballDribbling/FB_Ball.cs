using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class FB_Ball : MonoBehaviour
{
    private float ballOutOfFieldTimeOut;
    private Vector3 speed;
    private Vector3 previousPosition;
    private new Rigidbody rigidbody;
    private const float BALL_GROUND_POSITION_Y = 0.72f;
    private bool isWithPlayer;
    private bool isReachGoal=false;
    [SerializeField] private FB_GameManager manager;
    public float BallOutOfFieldTimeOut { get => ballOutOfFieldTimeOut; set => ballOutOfFieldTimeOut = value; }
    public Rigidbody Rigidbody { get => rigidbody; set => rigidbody = value; }
    public Vector3 Speed { get => speed; set => speed = value; }

    private FB_PlayerController player;
    private FB_PenaltyShooter penaltyShooter;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        isWithPlayer = false;
        penaltyShooter = GetComponent<FB_PenaltyShooter>();
    }
    public void StartPenalty()
    {
        penaltyShooter.StartPenalty();
    }
    public void PutOnGround()
    {
        transform.position = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, transform.position.z);
    }
    public void PutOnCenterSpot()
    {
        transform.position = new Vector3(-0.1f, BALL_GROUND_POSITION_Y, -0.049f);
    }

   
    // Update is called once per frame
    void Update()
    {
        if (!manager.IsOver)
        {
            if (transform.position.y < 0)
            {
                int error = 1;
            }
           
            if (isWithPlayer)
            {
                //Debug.Log("withPlayer");
                transform.position = player.PlayerBallPosition.position;
            }

            UpdateBallSpeedAndRotation();
        }
    }
    public void SetBallWithPlayer(bool isWithPlayer,FB_PlayerController player)
    {
        this.isWithPlayer = isWithPlayer;
        this.player = player;
    }
    public void Shoot(Vector3 shootDirection, Transform kickPoint)
    {

        Debug.Log("Shoot");

        isWithPlayer = false;

        // Đặt bóng tại vị trí chân trước khi sút
        transform.position = kickPoint.position;
        rigidbody.AddForce(shootDirection, ForceMode.Impulse);
        if (manager.GetKeeper() != null)
        {
            manager.GetKeeper().ReactToShot(shootDirection);
        }
        StartCoroutine(SetStateAfterShoot());

    }
    public IEnumerator SetStateAfterShoot()
    {
        yield return new WaitForSeconds(2f);
        if (!isReachGoal)
        {
            manager.GameOver(false);
        }
    }
    private void UpdateBallSpeedAndRotation()
    {
        if (Time.deltaTime > 0)
        {
            speed = new Vector3((transform.position.x - previousPosition.x) / Time.deltaTime, 0, (transform.position.z - previousPosition.z) / Time.deltaTime);
        }
        previousPosition.x = transform.position.x;
        previousPosition.z = transform.position.z;
        Vector3 rotationAxis = Vector3.Cross(speed.normalized, Vector3.up);
        transform.Rotate(rotationAxis, -speed.magnitude * 1.8f, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Va cham");
            manager.OnHitObstacle();
        }
        else if (other.CompareTag("Goal"))
        {
            isReachGoal = true;
            manager.OnReachGoal();
        }
    }
   
}
