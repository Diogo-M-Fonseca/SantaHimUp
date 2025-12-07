using UnityEngine;

public class EnemyTracker : MonoBehaviour
{
    private Level level;

    public void SetLevel(Level levelRef)
    {
        level = levelRef;
    }

    private void OnDestroy()
    {
        if (level != null)
        {
            level.RemoveEnemy(gameObject);
        }
    }
}
