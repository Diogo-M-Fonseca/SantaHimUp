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
    [SerializeField] private float attackPointDistance = 1.0f;
    [SerializeField] private float attackDamage = 12f;
    [SerializeField] private float attackCooldown = 0.3f;
    private bool canAttack = true;
    [SerializeField] private float knockbackMultiplier = 1.5f;

    [Header("Camera")]
    [SerializeField] private Camera cam;

    [Header("Animation")]
    [SerializeField] private Animator anim;

    private bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (cam == null)
            cam = Camera.main;

        if (anim == null)
            anim = GetComponent<Animator>();
    }

    void Update()
    {
        HandleInput();
        FaceMouse();
        UpdateAttackPoint();

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            StartAttack();
        }

        // Set walking animation
        bool isWalking = input.sqrMagnitude > 0.1f && !isAttacking;
        anim.SetBool("isWalking", isWalking);
    }

    void FixedUpdate()
    {
        if (!isAttacking)
            rb.linearVelocity = input.normalized * moveSpeed;
        else
            rb.linearVelocity = Vector2.zero; // Stop movement while attacking
    }

    void HandleInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    void FaceMouse()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = mousePos - transform.position;

        sr.flipX = dir.x < 0;
    }

    void UpdateAttackPoint()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (mousePos - transform.position).normalized;

        attackPoint.position = (Vector2)transform.position + dir * attackPointDistance;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        attackPoint.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void StartAttack()
    {
        canAttack = false;
        isAttacking = true;

        // Play attack animation
        anim.SetTrigger("Attack");

        // Perform attack logic
        Attack();

        // Reset attack after cooldown
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void Attack()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 attackDir = (mousePos - transform.position).normalized;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, 0.7f);
        foreach (Collider2D col in hitEnemies)
        {
            if (col.CompareTag("Enemy"))
            {
                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Vector2 knockDir = attackDir * knockbackMultiplier;
                    enemy.TakeDamage(attackDamage, knockDir);
                }
            }
        }
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