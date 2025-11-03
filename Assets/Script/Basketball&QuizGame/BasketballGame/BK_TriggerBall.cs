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
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
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
                BK_HooperMoving.Instance.SetLevel(level + 1);
                Debug.Log("Success");
                UIManager_.Instance.Open<PopupMessage>().Show("Level " + level.ToString(), 0.5f);
            }

        }


    }
    private IEnumerator ReenableTrigger(GameObject triggerObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        triggerObject.SetActive(true);

    }

}
