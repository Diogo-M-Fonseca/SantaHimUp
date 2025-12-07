
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Assign the Player Transform manually")]
    [SerializeField] private Transform player;
    private Rigidbody2D rb;

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
    [SerializeField] private float knockbackForce = 10f;

    [Header("Stun")]
    [SerializeField] private float stunDuration = 0.3f;
    private bool isStunned = false;
    private float stunEndTime;

    [Header("Hit Flash")]
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float flashDuration = 0.5f;
    private Color originalColor;

    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;

    public bool IsAlive { get; private set; } = true;
    private LevelDesign levelDesign;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        anim = GetComponent<Animator>();

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            PlayerHealth ph = FindObjectOfType<PlayerHealth>();
            if (ph != null)
                player = ph.transform;
            else
                Debug.LogWarning($"{name} could not find PlayerHealth in scene!");
        }

        originalColor = sr.color;
        
        levelDesign = FindObjectOfType<LevelDesign>();
        if (levelDesign != null)
            levelDesign.RegisterEnemy(this);
    }

    void FixedUpdate()
    {
        if (isStunned)
        {
            rb.linearVelocity = Vector2.zero;
            if (Time.time >= stunEndTime)
                isStunned = false;
            return;
        }

        if (player == null)
        {
            //anim.SetBool("isMoving", false);
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
                //anim.SetBool("isMoving", false);
                rb.linearVelocity = Vector2.zero;
                break;

            case State.Chase:
                ChasePlayer();
                break;

            case State.Attack:
                Attack();
                break;
        }

        transform.localScale = new Vector3(
            player.position.x > transform.position.x ? 1 : -1,
            1,
            1
        );
    }

    void ChasePlayer()
    {
        //anim.SetBool("isMoving", true);
        Vector2 dir = (player.position - transform.position).normalized;

        rb.linearVelocity = new Vector2(
            dir.x * moveSpeed,
            rb.linearVelocity.y
        );
    }

    void Attack()
    {
        //anim.SetBool("isMoving", false);
        rb.linearVelocity = Vector2.zero;

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            //anim.SetTrigger("attack");
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

        isStunned = true;
        stunEndTime = Time.time + stunDuration;
        rb.AddForce(knockDir.normalized * knockbackForce, ForceMode2D.Impulse);

        FlashHit();

        if (currentHealth <= 0)
            Die();
    }

    private void FlashHit()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    private System.Collections.IEnumerator FlashCoroutine()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        sr.color = originalColor;
    }

    public void Die()
    {
<<<<<<< HEAD
        IsAlive = false;
        if (levelDesign != null)
            levelDesign.UnregisterEnemy(this);
        
        // Add your death animation/effect here
=======
        anim.SetTrigger("die");
>>>>>>> cb472d1f38fc01340ba350edacbd8f5abb95bc31
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
        Destroy(gameObject, 2f);
    }
}
