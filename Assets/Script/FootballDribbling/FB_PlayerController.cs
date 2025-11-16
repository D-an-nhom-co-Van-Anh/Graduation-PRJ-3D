using UnityEngine;
using UnityEngine.InputSystem;

public class FB_PlayerController : MonoBehaviour
{
    [SerializeField] private FB_GameManager manager;
    [SerializeField] private PlayerAnimationController animController;
    int number;
    private const int LAYER_SHOOT = 1;
    private const int LAYER_CHEER = 2;
    private PlayerMovementController fellowPlayer;
    private FB_Ball scriptBall;
    private Rigidbody rigidbodyBall;
    private PlayerInput playerInput;
    private Transform transformBall;
    [SerializeField]private Transform playerBallPosition;
    private Transform playerCameraRoot;
    private Vector3 initialPosition;
    //private Animator animator;
    private AudioSource soundDribble;
    private AudioSource soundShoot;
    private AudioSource soundSteal;
    private float distanceSinceLastDribble;
    private bool hasBall;
    private float timeShot;
    private float stealDelay;       // after the player has lost the ball, he cannot steal it back for some time
    private float cheering;
    private float updateTime;
    //private Team team;
    private bool takeFreeKick;
    private bool takeThrowIn;
    private float shootingPower;
    private PlayerController playerController;
    //temp
    public float maxPower = 30f;         // Lực sút tối đa
    public float chargeSpeed = 20f;      // Tốc độ nạp lực
    public float upForce = 0.3f;         // Tỷ lệ nâng bóng (độ cao cú sút)

    private float currentPower = 0f;
    private bool charging = false;
    private Vector3 shootDirection;

    bool inPenaltyMode = false;
    GameObject penaltyTarget;
    FB_Ball ballRef;

    public bool HasBall { get => hasBall; set => hasBall = value; }
    public Transform PlayerBallPosition { get => playerBallPosition; set => playerBallPosition = value; }
    public Transform PlayerCameraRoot { get => playerCameraRoot; set => playerCameraRoot = value; }
   // public Team Team { get => team; set => team = value; }
    public PlayerMovementController FellowPlayer { get => fellowPlayer; set => fellowPlayer = value; }
    public Vector3 InitialPosition { get => initialPosition; set => initialPosition = value; }
    public bool TakeFreeKick { get => takeFreeKick; set => takeFreeKick = value; }
    public bool TakeThrowIn { get => takeThrowIn; set => takeThrowIn = value; }
    public int Number { get => number; set => number = value; }
    public PlayerInput PlayerInput { get => playerInput; set => playerInput = value; }
    public float ShootingPower { get => shootingPower; set => shootingPower = value; }

    void Awake()
    {
        transformBall = GameObject.Find("Ball").transform;
        scriptBall = transformBall.GetComponent<FB_Ball>();
        //soundDribble = GameObject.Find("Sound/dribble").GetComponent<AudioSource>();
        //soundShoot = GameObject.Find("Sound/shoot").GetComponent<AudioSource>();
        //soundSteal = GameObject.Find("Sound/woosh").GetComponent<AudioSource>();
        //animator = GetComponent<Animator>();
        //PlayerBallPosition = transform.Find("BallPosition");
        rigidbodyBall = transformBall.gameObject.GetComponent<Rigidbody>();
        //playerInput = GetComponent<PlayerInput>();
       // PlayerCameraRoot = transform.Find("PlayerCameraRoot");
        initialPosition = transform.position;
        playerController = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cheering > 0)
        {
            cheering -= Time.deltaTime;
        }
        if (stealDelay > 0)
        {
            stealDelay -= Time.deltaTime;
        }

        updateTime += Time.deltaTime;
        if (updateTime > 1.0f)
        {
            updateTime = 0f;
        }

        if (HasBall)
        {
            DribbleWithBall();
        }
       
        if (timeShot > 0)
        {
            // shoot ball
            if (HasBall/* && Time.time - timeShot > 0.2*/)
            {
                TakeShot();
            }
            // finished kicking animation
            if (Time.time - timeShot > 0.5)
            {
                timeShot = 0;
            }
        }
        else
        {
            //animator.SetLayerWeight(LAYER_SHOOT, Mathf.Lerp(animator.GetLayerWeight(LAYER_SHOOT), 0f, Time.deltaTime * 10f));
        }
    }


    private void DribbleWithBall()
    {
        transformBall.position = PlayerBallPosition.position;
        distanceSinceLastDribble += scriptBall.Speed.magnitude * Time.deltaTime;
        if (distanceSinceLastDribble >2)
        {
           // soundDribble.Play();
            distanceSinceLastDribble = 0;
        }
    }

    public void LooseBall(bool stolen = false)
    {
        if (stolen)
        {
            stealDelay = 2.0f;
        }
        HasBall = false;
        shootingPower = 0;
    }

    public void ScoreGoal()
    {
        cheering = 2.0f;
        //animator.SetLayerWeight(LAYER_CHEER, 1f);
    }

    public void Shoot()
    {
        if (HasBall)
        {
            Debug.Log("Shoot");
            timeShot = Time.time;
            //animator.Play("Shoot", LAYER_SHOOT, 0f);
            //animator.SetLayerWeight(LAYER_SHOOT, 1f);
        }
    }

    private void TakeShot()
    {
        //soundShoot.Play();
        //Game.Instance.SetPlayerWithBall(null);
        Debug.Log("Shoot");
        Vector3 shootdirection = transform.forward;
        shootdirection.y += 0.2f;
       // rigidbodyBall.AddForce(shootdirection * (10 + shootingPower * 20f), ForceMode.Impulse);
        LooseBall();
    }


    public void Activate()
    {
        //Game.Instance.ActiveHumanPlayer = this;
        playerInput.enabled = true;
    }

    public void SetPosition(Vector3 position)
    {
        if (
            //Team.IsHuman
            true
            )
        {
            GetComponent<CharacterController>().enabled = false;
            transform.position = position;
            GetComponent<CharacterController>().enabled = true;
        }
        else
        {
            transform.position = position;
        }
    }
    public void TakeBall()
    {
        hasBall = true;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.GetComponent<FB_Ball>())
        {
            TakeBall();
            scriptBall.SetBallWithPlayer(true, this);
        }
       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            playerController.LockMovement();
            manager.StartPenaltyMode(this);
        }
    }


    public void EnterPenaltyMode(GameObject target, FB_Ball ball)
    {
        inPenaltyMode = true;
        penaltyTarget = target;
        ballRef = ball;

        // Khóa di chuyển tự do nếu cần
        //rb.linearVelocity = Vector3.zero;
    }

    public void AimAndShootPenalty(Vector3 dir)
    {
        // Quay nhân vật theo hướng camera
        //transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        animController.PlayShootAnim();
        // Nếu người chơi nhấn sút
        if (hasBall)
        {
            hasBall = false;
            LooseBall();
            scriptBall.Shoot(dir, playerBallPosition);
            ExitPenaltyMode();
        }
    }
    public PlayerController GetPlayerController()
    {
        return playerController;
    }
    void ExitPenaltyMode()
    {
        inPenaltyMode = false;
        penaltyTarget.SetActive(false);
        manager.ExitPenaltyMode();
       // playerController.UnLockMovement();
        // Chuyển camera lại góc 3 người
        // (có thể gọi từ PenaltyTrigger)
    }
}
