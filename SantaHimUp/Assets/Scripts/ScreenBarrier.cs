
using UnityEngine;

public class ScreenBarrier : MonoBehaviour
{
    private BoxCollider2D barrierCollider;
    private Rigidbody2D playerRigidbody;

    private void Start()
    {
        barrierCollider = GetComponent<BoxCollider2D>();
        if (barrierCollider != null)
        {
            barrierCollider.enabled = true;
            barrierCollider.isTrigger = true;
        }

        // Find the player by tag instead of serialized reference
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            playerRigidbody = playerObject.GetComponent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object has a Rigidbody2D (player characteristic)
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
        if (rb != null && collision.CompareTag("Player"))
        {
            UnlockNextArea();
        }
    }

    public void UnlockNextArea()
    {
        if (barrierCollider != null)
        {
            barrierCollider.enabled = false;
            Debug.Log("Barrier unlocked!");
        }
    }

    public void LockArea()
    {
        if (barrierCollider != null)
        {
            barrierCollider.enabled = true;
            Debug.Log("Barrier locked!");
        }
    }
}
