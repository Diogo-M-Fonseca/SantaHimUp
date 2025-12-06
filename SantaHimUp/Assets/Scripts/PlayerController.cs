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
    [SerializeField] private float attackDamage = 12f;
    [SerializeField] private float attackCooldown = 0.3f;
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float knockbackMultiplier = 1.5f;

    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleInput();

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            Attack();
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void HandleInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (input.x != 0)
            sr.flipX = input.x < 0;
    }

    void Move()
    {
        rb.linearVelocity = input.normalized * moveSpeed;
    }

    void Attack()
    {
        canAttack = false;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, 0.7f);

        foreach (Collider2D col in hitEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Vector2 knockDir =
                        (col.transform.position - transform.position).normalized
                        * knockbackMultiplier;

                    enemy.TakeDamage(attackDamage, knockDir);
                }
            }
        }

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, 0.8f);
    }
}