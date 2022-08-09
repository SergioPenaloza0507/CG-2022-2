using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackController : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    private Animator animator { get; set; }

    private int attackHash;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        attackHash = Animator.StringToHash("Attack");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger(attackHash);
            if(particles != null)
                particles.Play(true);
        }
    }
}
