using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    private Vector2 input;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    [Header("Combat")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackDamage = 12f;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Animation")]
    [SerializeField] private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();

        if (Input.GetKeyDown(KeyCode.J))
            TryAttack();
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        // Flip only horizontally
        if (input.x != 0)
            sr.flipX = input.x < 0;
    }

    void Move()
    {
        rb.linearVelocity = input.normalized * moveSpeed;
    }

    void HandleAnimation()
    {
        anim.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    void TryAttack()
    {
        if (!canAttack) return;

        anim.SetTrigger("Attack");
        canAttack = false;

        // Delay hit to match animation
        Invoke(nameof(DoAttack), 0.1f);
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void DoAttack()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                
                Vector2 dir = (hit.transform.position - transform.position).normalized;

                enemy.TakeDamage(attackDamage, dir);
            }
        }
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
