using System.Collections;
using UnityEngine;

public class BK_HooperMoving : MonoBehaviour
{
    public static BK_HooperMoving Instance { get; private set; }

    [SerializeField] private float minZ = 0.5f;
    [SerializeField] private float maxZ = 4.8f;
    [SerializeField] private float baseSpeed = 1f;
    [SerializeField] private float speedIncreasePerLevel = 0.3f;
    [SerializeField] private int maxLevel = 10;
    [SerializeField] private float returnSpeed = 1f;

    private Coroutine returnRoutine;

    private int level = 1;

    private Vector3 startPos;

    private float moveSpeed;
    private float centerZ;
    private float maxAmplitude;
    private float currentReturnSpeed;
    private float elapsedTime;

    private bool isMoving = false;
    private bool canStartMoving = false;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        centerZ = (minZ + maxZ) / 2f;
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, centerZ);
        startPos = transform.localPosition;
        maxAmplitude = (maxZ - minZ) / 2f;
        UpdateSpeed();
    }

    void Update()
    {
        if (!isMoving) return;

        if (level > 1)
        {
            elapsedTime += Time.deltaTime;
            float amplitudeFactor = Mathf.InverseLerp(1, maxLevel, level);
            if (level == 1) amplitudeFactor = 0f;
            float amplitude = Mathf.Lerp(0f, maxAmplitude, amplitudeFactor);

            float zPos = centerZ + Mathf.Sin(elapsedTime * moveSpeed) * amplitude;
            transform.localPosition = new Vector3(startPos.x, startPos.y, zPos);
        }
        else
        {
            transform.localPosition = new Vector3(startPos.x, startPos.y, centerZ);
        }
    }
    public int GetLevel()
    {
        return level;

    }
    public void SetLevel(int newLevel)
    {
        level = newLevel;
        UpdateSpeed();
    }

    private void UpdateSpeed()
    {
        if (level > 1)
            moveSpeed = baseSpeed + (level - 2) * speedIncreasePerLevel;
        else
            moveSpeed = 0f;
    }
    public void CanStartMoving()
    {
        canStartMoving = true;
    }
    public void StartMoving()
    {
        if (canStartMoving)
        {
            isMoving = true;
            elapsedTime = 0f;
            transform.localPosition = startPos;
            currentReturnSpeed = moveSpeed > 0 ? moveSpeed : returnSpeed;
            canStartMoving = false;
        }
    }
    public void StopMoving()
    {
        isMoving = false;
        if (returnRoutine != null)
            StopCoroutine(returnRoutine);
        returnRoutine = StartCoroutine(DelayThenReturn());
    }

    private IEnumerator DelayThenReturn()
    {
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ReturnToCenter());
    }
    private IEnumerator ReturnToCenter()
    {
        Vector3 pos = transform.localPosition;
        while (Mathf.Abs(pos.z - centerZ) > 0.001f)
        {
            pos = transform.localPosition;
            pos.z = Mathf.MoveTowards(pos.z, centerZ, currentReturnSpeed * Time.deltaTime);
            transform.localPosition = pos;
            yield return null;
        }
        pos.z = centerZ;
        transform.localPosition = pos;
        returnRoutine = null;
    }
}
