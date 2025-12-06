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
    [SerializeField] private float knockbackForce = 4f;

    //[Header("Animation")]
    //[SerializeField] private Animator anim;

    private bool isAttacking = false;

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
        //anim.SetFloat("Speed", rb.linearVelocity.magnitude);
    }

    void TryAttack()
    {
        if (!canAttack) return;

        canAttack = false;
        isAttacking = true;
        //anim.SetTrigger("Attack");

        Invoke(nameof(EndAttack), attackCooldown);
    }

    void EndAttack()
    {
        canAttack = true;
        isAttacking = false;
    }

    private void OnMouseDown()
    {
        if (!canAttack) return;
        else if (Input.GetMouseButtonDown(0))
        {
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isAttacking) return; 

        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
          
            Vector2 knockDir = (collision.transform.position - transform.position).normalized;
            enemy.TakeDamage(attackDamage, knockDir * knockbackForce);
        }
    }
}