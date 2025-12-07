
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LevelDesign : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies = new List<Enemy>();
    [SerializeField] private TextMeshProUGUI goText;
    [SerializeField] private ScreenBarrier screenBarrier;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform player;
    [SerializeField] private float cameraXPositions;
    [SerializeField] private float cameraMoveSpeed = 5f;
    [SerializeField] private float goFlickerSpeed = 0.5f;

    private bool allEnemiesDead = false;
    private int currentStageIndex = 0; 

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        if (goText != null)
            goText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CheckEnemiesStatus();
    }

    private void CheckEnemiesStatus()
    {
        if (allEnemiesDead)
            return;

        bool allDead = true;
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null && enemy.IsAlive)
            {
                allDead = false;
                break;
            }
        }

        if (allDead && enemies.Count > 0)
        {
            allEnemiesDead = true;
            OnAllEnemiesDead();
        }
    }

    private void OnAllEnemiesDead()
    {
        if (goText != null)
            StartCoroutine(FlickerGOText());

        if (screenBarrier != null)
            screenBarrier.UnlockNextArea();

        if (mainCamera != null)
            StartCoroutine(MoveCameraToNextStage());
    }

    private IEnumerator FlickerGOText()
    {
        goText.gameObject.SetActive(true);
        float elapsedTime = 0f;
        float flickerDuration = 2f;

        while (elapsedTime < flickerDuration)
        {
            goText.alpha = Mathf.PingPong(elapsedTime * goFlickerSpeed, 1f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        goText.gameObject.SetActive(false);
    }

    private IEnumerator MoveCameraToNextStage()
    {
        Vector3 targetPosition = new Vector3(cameraXPositions, mainCamera.transform.position.y, mainCamera.transform.position.z);

        while (Vector3.Distance(mainCamera.transform.position, targetPosition) > 0.01f)
        {
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosition, Time.deltaTime * cameraMoveSpeed);
            
            // Move player along with camera
            if (player != null)
            {
                player.position = new Vector3(mainCamera.transform.position.x, player.position.y, player.position.z);
            }
            
            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        
        // Final player position adjustment
        if (player != null)
        {
            player.position = new Vector3(cameraXPositions, player.position.y, player.position.z);
        }
        
        currentStageIndex++;
        allEnemiesDead = false;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}
