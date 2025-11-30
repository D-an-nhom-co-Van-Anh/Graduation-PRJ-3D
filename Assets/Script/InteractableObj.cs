using UnityEngine;

public class InteractableObj : MonoBehaviour
{
    public virtual void Interact()
    {
        Debug.Log("Interact");
    }
}