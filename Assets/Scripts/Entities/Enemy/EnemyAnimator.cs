using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;

    private readonly int Die = Animator.StringToHash("Die");

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetDeathAnimation()
    {
        animator.SetTrigger(Die);
    }
}
