using System.Collections;
using UnityEngine;

public class ThrowBasketball : MonoBehaviour
{
    public GameObject ball;
    private PlayerCameraSwitcher cameraSwitcher;

    [SerializeField] private float gravityForce = 40f;
    [SerializeField] private float throwAngle = 35f;
    [SerializeField] private int energyCost;
    [SerializeField] private float resetTime = 30f;
    [SerializeField] private float minForce = 10f;
    [SerializeField] private float maxForce = 60f;

    private Vector3 holdPoint = new Vector3(0.9f, -0.8f, 1.8f);
    private Vector3 maxHoldPoint = new Vector3(0.9f, -0.96f, 1.5f);
    private Vector3 originPosition = new Vector3(158.03f, 0.627f, 38.03f);

    private Rigidbody rb;

    private bool isHoldingBall = false;
    private bool isPreparingThrow = false;
    private bool isMovingToHand = false;
    private int level = 1;
    private float prepareProgress = 0f;
    private float prepareSpeed = 1f;
    private float throwForce;
    private Coroutine resetCorountine;
    private TriggerBall triggerBall;
    private LineRenderer lineRenderer;
    private BasketballForceUI forceUI;
    public void Start()
    {
        triggerBall = gameObject.GetComponent<TriggerBall>();
        GameObject targetObj = GameObject.Find("BasketballTarget");
        cameraSwitcher = targetObj.GetComponent<PlayerCameraSwitcher>();
        rb = ball.GetComponent<Rigidbody>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        rb.useGravity = false;
        throwForce = 25f;
    }
    public void Update()
    {
        if (cameraSwitcher.IsFirstPersonView() == true)
        {
            if (isHoldingBall)
            {
                forceUI = UIManager_.Instance.Open<BasketballForceUI>();
                forceUI.ShowForceUI();

                if (Input.GetKey(KeyCode.F))
                {
                    if (!isPreparingThrow)
                    {
                        isPreparingThrow = true;
                        prepareProgress = 0f;
                        
                    }

                    
                     prepareProgress += Time.deltaTime * prepareSpeed;
                    prepareProgress = Mathf.Clamp01(prepareProgress);

                    forceUI.UpdateForce(prepareProgress);

                    throwForce = Mathf.Lerp(minForce, maxForce, prepareProgress);
                    Vector3 currentHoldPos = Vector3.Lerp(holdPoint, maxHoldPoint, prepareProgress);
                    ball.transform.localPosition = currentHoldPos;
                }
                else if (isPreparingThrow && Input.GetKeyUp(KeyCode.F))
                {
                    isPreparingThrow = false;
                    Throw();
                    UIManager_.Instance.CloseDirect<BasketballForceUI>();
                    prepareProgress = 0f;
                    throwForce = 25f;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.F))
                {
                    GetTheBallFromGround();
                }
            }
        }
    }
    public void FixedUpdate()
    {
        if (!isHoldingBall && !rb.isKinematic)
        {
            Vector3 customGravity = new Vector3(0, -gravityForce, 0);
            rb.AddForce(customGravity, ForceMode.Acceleration);
        }
      


    }
    private void LateUpdate()
    {
        if (isHoldingBall)
        {

            //tinh toan duong ve Line renderer
            int points = 50;
            float timeStep = 0.1f;
            Vector3 start = ball.transform.position + Camera.main.transform.right * 0.2f;
            Vector3 velocity = CalculateThrowDirection() * throwForce;

            lineRenderer.positionCount = points;

            for (int i = 0; i < points; i++)
            {
                float t = i * timeStep;
                Vector3 point = start + velocity * t + 0.5f * new Vector3(0, -gravityForce, 0) * t * t;
                lineRenderer.SetPosition(i, point);
            }
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;

        }
    }

    public void OnInteract()
    {

    }
    public bool IsInteractable()
    {
        return true;
    }
    public void GetTheBallFromGround()
    {
        if (isHoldingBall || isMovingToHand) return;
        StartCoroutine(MoveBallToHand());
        level = BK_HooperMoving.Instance.GetLevel();
        BK_HooperMoving.Instance.StartMoving();
        AudioManager.Instance.PlaySFX("getBall");
        
    }
    private IEnumerator MoveBallToHand()
    {
        isMovingToHand = true;
        triggerBall.DisableTrigger();
        Transform playerGroup = Camera.main?.transform;
        Vector3 targetPos;
        Quaternion targetRot;

        rb.isKinematic = true;
        rb.useGravity = false;

        float duration = 0.2f;
        float elapsed = 0f;

        Vector3 startPos = ball.transform.position;
        Quaternion startRot = ball.transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            targetPos = playerGroup.TransformPoint(new Vector3(0.3f, -0.8f, 0.5f));
            targetRot = Quaternion.identity;
            ball.transform.position = Vector3.Lerp(startPos, targetPos, t);
            ball.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        ball.transform.SetParent(playerGroup, false);
        ball.transform.localPosition = holdPoint;
        ball.transform.localRotation = Quaternion.identity;

        triggerBall.EnableTrigger();
        isHoldingBall = true;
        isMovingToHand = false;
    }
    public void Throw()
    {
        Transform yard = GameObject.Find("Map").transform;
        Vector3 originalScale = ball.transform.localScale;

        ball.transform.SetParent(yard, true);
        ball.transform.localScale = originalScale;

        rb.isKinematic = false;

        rb.AddForce(CalculateThrowDirection() * throwForce, ForceMode.Impulse);
        isHoldingBall = false;
        AudioManager.Instance.PlaySFX("throwBall");

        gameObject.layer = LayerMask.NameToLayer("Default");
        if (resetCorountine != null) StopCoroutine(resetCorountine);
        resetCorountine = StartCoroutine(ResetBallAfterDelay());
    }

    private Vector3 CalculateThrowDirection()
    {
        Vector3 cameraForward = Camera.main.transform.forward.normalized;
        Quaternion pitchRotation = Quaternion.AngleAxis(-throwAngle, Camera.main.transform.right);
        return pitchRotation * cameraForward;
    }
    private IEnumerator ResetBallAfterDelay()
    {
        yield return new WaitForSeconds(resetTime);

        if (!isHoldingBall)
        {
            ball.transform.localPosition = originPosition;
        }
    }
    public void ResetBallPosition()
    {
        ball.transform.localPosition = originPosition;
    }
    public bool IsOnGround()
    {
        return transform.localPosition.y < 0.4f;
    }

    public bool IsFlying()
    {
        return transform.localPosition.y >= 0.4f;

    }
}
