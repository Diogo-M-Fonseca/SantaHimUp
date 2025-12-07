using UnityEngine;
using UnityEngine.SceneManagement;

public class Santa : MonoBehaviour
{
    [Header("Enemy Launching")]
    public GameObject enemyPrefab;      
    public Transform handPoint;         
    public Transform player;            
    public float launchForce = 500f;    
    public float spawnInterval = 3f;    

    private float spawnTimer;
    [SerializeField] private float currentHealth;
    [SerializeField] private float maxHealth = 40;

    private void Start()
    {
        currentHealth = maxHealth;
    }
    void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            LaunchEnemy();
            spawnTimer = 0f;
        }
    }

    void LaunchEnemy()
    {
        if (enemyPrefab == null || player == null || handPoint == null) return;

        GameObject enemy = Instantiate(enemyPrefab, handPoint.position, Quaternion.identity);

        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 direction = (player.position - handPoint.position).normalized;

            rb.AddForce(direction * launchForce);
        }
    }

    public void TakeDamage(float dmg, Vector2 knockDir)
    {
        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        this.enabled = false;
        Destroy(gameObject, 2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
