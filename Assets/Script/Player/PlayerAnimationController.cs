using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField]private Animator animator;

    private static readonly int IsWalkingHash = Animator.StringToHash("Is Walking");
    private static readonly int IsRunningHash = Animator.StringToHash("Is Running");
    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private static readonly int VictoryTriggerHash = Animator.StringToHash("Victory");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Cập nhật trạng thái di chuyển theo input và chạy/đi bộ.
    /// </summary>
    public void UpdateMovement(Vector2 moveInput, bool isRunningInput)
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        // Logic chuyển state
        bool isWalking = isMoving && !isRunningInput;
        bool isRunning = isMoving && isRunningInput;

        animator.SetBool(IsWalkingHash, isWalking);
        animator.SetBool(IsRunningHash, isRunning);
    }

    public void PlayJump()
    {
        animator.ResetTrigger(VictoryTriggerHash);
        animator.SetTrigger(JumpTriggerHash);
    }

    public void PlayVictory()
    {
        animator.SetTrigger(VictoryTriggerHash);
    }
}
