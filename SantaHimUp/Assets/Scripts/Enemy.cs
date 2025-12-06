using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Assign the Player Transform manually")]
    [SerializeField] private Transform player;   
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator anim;

    [Header("Stats")]
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float currentHealth;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float chaseDistance = 6f;
    [SerializeField] private float attackDistance = 1.2f;

    [Header("Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float lastAttackTime;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 4f;

    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (player == null)
        {
            anim.SetBool("isMoving", false);
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= attackDistance)
            currentState = State.Attack;
        else if (dist <= chaseDistance)
            currentState = State.Chase;
        else
            currentState = State.Idle;

        switch (currentState)
        {
            case State.Idle:
                anim.SetBool("isMoving", false);
                rb.linearVelocity = Vector2.zero;
                break;

            case State.Chase:
                ChasePlayer();
                break;

            case State.Attack:
                Attack();
                break;
        }
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);
    }

    void ChasePlayer()
    {
        anim.SetBool("isMoving", true);
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);
    }

    void Attack()
    {
        anim.SetBool("isMoving", false);
        rb.linearVelocity = Vector2.zero;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            anim.SetTrigger("attack");

            DealDamage(); 
        }
    }

    void DealDamage()
    {
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist <= attackDistance + 0.2f)
        {
            player.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(float dmg, Vector2 knockDir)
    {
        currentHealth -= dmg;
        rb.AddForce(knockDir.normalized * knockbackForce, ForceMode2D.Impulse);

        anim.SetTrigger("hit");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        anim.SetTrigger("die");
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
        Destroy(gameObject, 2f);
    }
}