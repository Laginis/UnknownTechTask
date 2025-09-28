using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;

    private readonly int Run = Animator.StringToHash("Run");
    private readonly int Attack = Animator.StringToHash("Attack");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetRunAnimation()
    {
        animator.SetBool(Run, true);
    }

    public void SetAttackAnimation()
    {
        animator.SetTrigger(Attack);
    }

    public void SetIdleAnimation()
    {
        animator.SetBool(Run, false);
    }
}
