using UnityEngine;

public class NpcAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetTalking(bool isTalking)
    {
        if (animator == null) return;

        animator.SetBool("Is Talking", isTalking);
    }
}
