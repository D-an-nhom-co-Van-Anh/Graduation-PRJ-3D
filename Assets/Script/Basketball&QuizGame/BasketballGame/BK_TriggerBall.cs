using UnityEngine;
using System.Collections;
using System;

public class TriggerBall : MonoBehaviour
{
    [SerializeField] private string bottomTriggerName = "TriggerBall";
    private int level = 0;
    private string invalidPosition = "InvalidPosition";
    [SerializeField] private float resetTime = 0.5f;
    private Rigidbody rb;

    private SphereCollider ballCollider;
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        ballCollider = GetComponent<SphereCollider>();
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name != bottomTriggerName && other.gameObject.name != invalidPosition) return;
        if (other.gameObject.name == invalidPosition)
        {
            other.gameObject.SetActive(false);
            GameObject bottomTrigger = GameObject.Find(bottomTriggerName);
            if (bottomTrigger != null)
            {
                bottomTrigger.SetActive(false);
                StartCoroutine(ReenableTrigger(bottomTrigger, resetTime));
            }

            StartCoroutine(ReenableTrigger(other.gameObject, resetTime));

        }
        else if (other.gameObject.name == bottomTriggerName)
        {
            if (rb.linearVelocity.y <= 0f)
            {
                BK_HooperMoving.Instance.CanStartMoving();
                BK_HooperMoving.Instance.StopMoving();
                level = BK_HooperMoving.Instance.GetLevel();
                level += 1;
                BK_HooperMoving.Instance.SetLevel(level);
                UIManager_.Instance.Open<PopupMessage>().Show("Level " + level.ToString(), 0.5f);
            }

        }


    }
    private IEnumerator ReenableTrigger(GameObject triggerObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        triggerObject.SetActive(true);

    }
    public void EnableTrigger()
    {
            ballCollider.enabled=true;
    }

    // ?? Hàm t?t trigger (chuy?n Collider v? va ch?m v?t lý bình th??ng)
    public void DisableTrigger()
    {
      
            ballCollider.enabled = false;
    }
}
