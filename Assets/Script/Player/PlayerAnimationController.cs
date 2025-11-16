using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int IsWalkingHash = Animator.StringToHash("Is Walking");
    private static readonly int IsRunningHash = Animator.StringToHash("Is Running");
    private static readonly int JumpTriggerHash = Animator.StringToHash("Jump");
    private static readonly int VictoryTriggerHash = Animator.StringToHash("Victory");

    // Trạng thái cũ để kiểm tra thay đổi
    private bool lastIsWalking;
    private bool lastIsRunning;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Cập nhật trạng thái di chuyển theo input và chạy/đi bộ.
    /// Chỉ cập nhật Animator khi có thay đổi thật sự để tránh khựng animation.
    /// </summary>
    public void UpdateMovement(Vector2 moveInput, bool isRunningInput)
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        bool isWalking = isMoving && !isRunningInput;
        bool isRunning = isMoving && isRunningInput;

        // Chỉ set khi có thay đổi trạng thái thật sự
        if (isWalking != lastIsWalking)
        {
            animator.SetBool(IsWalkingHash, isWalking);
            lastIsWalking = isWalking;
        }

        if (isRunning != lastIsRunning)
        {
            animator.SetBool(IsRunningHash, isRunning);
            lastIsRunning = isRunning;
        }
    }

    /// <summary>
    /// Phát hoạt ảnh nhảy (chặn spam jump).
    /// </summary>
    public void PlayJump()
    {
        if (CanPlayJump())
        {
            animator.ResetTrigger(VictoryTriggerHash);
            animator.SetTrigger(JumpTriggerHash);
        }
    }

    public void EndJump()
    {
        animator.SetBool("Jump", false); // hoặc SetTrigger("Land") nếu bạn dùng trigger
    }

    /// <summary>
    /// Chỉ cho phép nhảy nếu chưa trong animation Jump.
    /// </summary>
    public bool CanPlayJump()
    {
        var state = animator.GetCurrentAnimatorStateInfo(0);
        return !state.IsName("Jump") && !animator.IsInTransition(0);
    }

    public void PlayVictory()
    {
        animator.SetTrigger(VictoryTriggerHash);
    }
    public void PlayShootAnim()
    {
        animator.SetTrigger("Shoot");
    }
}
