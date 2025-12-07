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
    [SerializeField] private bool canAttack = true;
    [SerializeField] private float knockbackMultiplier = 1.5f;

    [Header("Camera")]
    [SerializeField] private Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (cam == null)
            cam = Camera.main;
    }

    void Update()
    {
        HandleInput();
        FaceMouse();
        UpdateAttackPoint();

        if (Input.GetMouseButtonDown(0) && canAttack)
            Attack();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = input.normalized * moveSpeed;
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

    void Attack()
    {
        canAttack = false;

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

        Invoke(nameof(ResetAttack), attackCooldown);
    }

    void ResetAttack()
    {
        canAttack = true;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, 0.8f);
    }
}