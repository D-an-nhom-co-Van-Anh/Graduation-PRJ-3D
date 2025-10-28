using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UIElements;

public class FB_Ball : MonoBehaviour
{
    private float ballOutOfFieldTimeOut;
    private bool isThrowIn;
    private Vector3 speed;
    private Vector3 ballOutOfFieldposition;
    private Vector3 previousPosition;
    private new Rigidbody rigidbody;
    private AudioSource soundWhistle;
    private const float BALL_GROUND_POSITION_Y = 0.72f;
    private const float PASSING_SPEED = 25f;
    private bool isWithPlayer;
    [SerializeField] private FB_GameManager manager;
    public float BallOutOfFieldTimeOut { get => ballOutOfFieldTimeOut; set => ballOutOfFieldTimeOut = value; }
    public Rigidbody Rigidbody { get => rigidbody; set => rigidbody = value; }
    public Vector3 Speed { get => speed; set => speed = value; }

    private float timePassedBall;
    private CinemachineCamera playerFollowCamera;
    private FB_PlayerController player;
    private void Awake()
    {
       // playerFollowCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineCamera>();
        //soundWhistle = GameObject.Find("Sound/whistle").GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody>();
        isWithPlayer = false;
    }

    public void Pass(PlayerMovementController player)
    {
        timePassedBall = Time.time;
        //Game.Instance.PassDestinationPlayer = player.FellowPlayer;
    }

    public void PutOnGround()
    {
        transform.position = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, transform.position.z);
    }
    public void PutOnCenterSpot()
    {
        transform.position = new Vector3(-0.1f, BALL_GROUND_POSITION_Y, -0.049f);
    }

    //private void TakeThrowIn()
    //{
    //    if (Game.Instance.PlayerWithBall != null)
    //    {
    //        Game.Instance.PlayerWithBall.HasBall = false;
    //        Game.Instance.SetPlayerWithBall(null);
    //    }
    //    transform.position = ballOutOfFieldposition;
    //    rigidbody.velocity = Vector3.zero;
    //    rigidbody.angularVelocity = Vector3.zero;
    //    Player player = Game.Instance.GetPlayerToThrowIn();
    //    player.SetPosition(new Vector3(ballOutOfFieldposition.x, player.transform.position.y, ballOutOfFieldposition.z));
    //    // Look in the directon of the field. Otherwise the ball will be out of field again.
    //    player.transform.LookAt(Game.Instance.KickOffPosition);
    //    Game.Instance.SetPlayerWithBall(player);
    //    if (isThrowIn)
    //    {
    //        player.TakeThrowIn = true;
    //    }
    //    else
    //    {
    //        player.TakeFreeKick = true;
    //    }
    //    // move players that are too close
    //    Game.Instance.SetMinimumDistanceOtherPlayers(player);
    //}

    //private void CheckBallOutOfField()
    //{
    //    // ball out of field
    //    if (transform.position.z < Game.FIELD_BOUNDARY_LOWER_Z)
    //    {
    //        soundWhistle.Play();
    //        isThrowIn = true;
    //        ballOutOfFieldTimeOut = 1.0f;
    //        ballOutOfFieldposition = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, -25);
    //    }
    //    if (transform.position.z > Game.FIELD_BOUNDARY_UPPER_Z)
    //    {
    //        soundWhistle.Play();
    //        isThrowIn = true;
    //        ballOutOfFieldTimeOut = 1.0f;
    //        ballOutOfFieldposition = new Vector3(transform.position.x, BALL_GROUND_POSITION_Y, 25);
    //    }

    //    if (transform.position.x < Game.FIELD_BOUNDARY_LOWER_X)
    //    {
    //        soundWhistle.Play();
    //        isThrowIn = false;
    //        ballOutOfFieldTimeOut = 1.0f;
    //        if (Game.Instance.TeamLastTouched == 0)
    //        {
    //            // goal kick
    //            ballOutOfFieldposition = new Vector3(-46.8f, BALL_GROUND_POSITION_Y, -4.89f);
    //        }
    //        else
    //        {
    //            // corner
    //            if (transform.position.z > 0)
    //            {
    //                ballOutOfFieldposition = new Vector3(-52.6f, BALL_GROUND_POSITION_Y, 25);
    //            }
    //            else
    //            {
    //                ballOutOfFieldposition = new Vector3(-52.6f, BALL_GROUND_POSITION_Y, -25);
    //            }
    //        }
    //    }
    //    if (transform.position.x > Game.FIELD_BOUNDARY_UPPER_X)
    //    {
    //        soundWhistle.Play();
    //        isThrowIn = false;
    //        ballOutOfFieldTimeOut = 1.0f;
    //        if (Game.Instance.TeamLastTouched == 1)
    //        {
    //            // goal kick
    //            ballOutOfFieldposition = new Vector3(46.58f, BALL_GROUND_POSITION_Y, 5.29f);
    //        }
    //        else
    //        {
    //            // corner
    //            if (transform.position.z > 0)
    //            {
    //                ballOutOfFieldposition = new Vector3(52.3f, BALL_GROUND_POSITION_Y, 25);
    //            }
    //            else
    //            {
    //                ballOutOfFieldposition = new Vector3(52.3f, BALL_GROUND_POSITION_Y, -25);
    //            }
    //        }
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < 0)
        {
            int error = 1;
        }

        if (ballOutOfFieldTimeOut > 0)
        {
            //ballOutOfFieldTimeOut -= Time.deltaTime;
            //if (ballOutOfFieldTimeOut <= 0 && !Game.Instance.WaitingForKickOff)
            //{
            //   // TakeThrowIn();
            //}
        }
        else
        {
            //CheckBallOutOfField();
        }

        //if (Game.Instance.PassDestinationPlayer != null)
        //{
        //    PassBall();
        //}
        //else if (Game.Instance.PlayerWithBall != null)
        //{
        //    transform.position = Game.Instance.PlayerWithBall.PlayerBallPosition.position;
        //}
        if (isWithPlayer)
        {
            Debug.Log("withPlayer");
            transform.position = player.PlayerBallPosition.position;
        }

        UpdateBallSpeedAndRotation();
    }
    public void SetBallWithPlayer(bool isWithPlayer,FB_PlayerController player)
    {
        this.isWithPlayer = isWithPlayer;
        this.player = player;
    }
    public void Shoot(Vector3 shootdirection)
    {
        Debug.Log("Shoot");
        isWithPlayer = false;
        rigidbody.AddForce(shootdirection * (10 + 1f * 1f), ForceMode.Impulse);
        
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
            manager.OnReachGoal();
        }
    }
    //private void PassBall()
    //{
    //    if (Time.time - timePassedBall > 0.2 && playerFollowCamera.Follow != Game.Instance.PassDestinationPlayer.PlayerCameraRoot)
    //    {
    //        // switch player
    //        Game.Instance.PassDestinationPlayer.FellowPlayer.PlayerInput.enabled = false;
    //        Game.Instance.PassDestinationPlayer.Activate();
    //        playerFollowCamera.Follow = Game.Instance.PassDestinationPlayer.PlayerCameraRoot;
    //    }

    //    Vector3 movedirection = Game.Instance.PassDestinationPlayer.PlayerBallPosition.position - transform.position;
    //    if (movedirection.magnitude < 1f)
    //    {
    //        // pass arrived
    //        transform.position = Game.Instance.PassDestinationPlayer.PlayerBallPosition.position;
    //        Game.Instance.SetPlayerWithBall(Game.Instance.PassDestinationPlayer);
    //    }
    //    movedirection.Normalize();
    //    transform.position += movedirection * PASSING_SPEED * Time.deltaTime;
    //}
}
