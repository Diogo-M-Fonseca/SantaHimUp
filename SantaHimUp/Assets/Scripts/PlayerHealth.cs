using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private PlayerController player;

    void Start()
    {
        currentHealth = maxHealth;
        player = GetComponent<PlayerController>();
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Debug.Log("Player Died!");
        
    }
}
