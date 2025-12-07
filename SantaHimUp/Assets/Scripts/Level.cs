using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        public string stageName;
        public int enemyCount;
        public Vector3 cameraPosition;
        public float cameraSize = 5f;
    }

    [SerializeField] private Stage[] stages = new Stage[3];
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float cameraMoveSpeed = 2f;
    [SerializeField] private float goFlickerSpeed = 0.2f;

    private int currentStage = 0;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private GameObject goIndicator;
    private bool isStageComplete = false;
    private bool isTransitioning = false;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        CreateGOIndicator();
        StartStage(0);
    }

    private void Update()
    {
        // Check if all enemies are defeated
        if (!isStageComplete && activeEnemies.Count == 0 && currentStage < stages.Length)
        {
            isStageComplete = true;
            StartCoroutine(ShowGOAndTransition());
        }
    }

    private void StartStage(int stageIndex)
    {
        if (stageIndex >= stages.Length)
        {
            Debug.Log("All stages completed!");
            return;
        }

        currentStage = stageIndex;
        isStageComplete = false;
        isTransitioning = false;
        activeEnemies.Clear();

        Debug.Log($"Starting Stage {currentStage + 1}: {stages[stageIndex].stageName}");

        // Spawn enemies for this stage
        for (int i = 0; i < stages[stageIndex].enemyCount; i++)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        if (enemyPrefab == null || enemySpawnPoint == null)
        {
            Debug.LogError("Enemy prefab or spawn point not assigned!");
            return;
        }

        Vector3 spawnPos = enemySpawnPoint.position + new Vector3(Random.Range(-2f, 2f), 0, 0);
        GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
        
        // Add a script to track enemy death
        EnemyTracker tracker = enemy.AddComponent<EnemyTracker>();
        tracker.SetLevel(this);

        activeEnemies.Add(enemy);
    }

    public void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    private void CreateGOIndicator()
    {
        GameObject goObject = new GameObject("GOIndicator");
        goObject.transform.SetParent(GameObject.Find("Canvas")?.transform ?? null);

        TextMesh textMesh = goObject.AddComponent<TextMesh>();
        textMesh.text = "GO";
        textMesh.fontSize = 100;
        textMesh.alignment = TextAlignment.Center;
        textMesh.anchor = TextAnchor.MiddleCenter;

        RectTransform rectTransform = goObject.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(300, 0);
        rectTransform.sizeDelta = new Vector2(200, 200);

        goIndicator = goObject;
        goIndicator.SetActive(false);
    }

    private IEnumerator ShowGOAndTransition()
    {
        isTransitioning = true;
        goIndicator.SetActive(true);

        // Flicker the GO indicator
        float elapsedTime = 0f;
        float flickerDuration = 1.5f;

        while (elapsedTime < flickerDuration)
        {
            elapsedTime += Time.deltaTime;
            bool isVisible = (elapsedTime / goFlickerSpeed) % 2 < 1;
            goIndicator.SetActive(isVisible);
            yield return null;
        }

        goIndicator.SetActive(false);

        // Move camera to next stage
        if (currentStage + 1 < stages.Length)
        {
            yield return StartCoroutine(MoveCamera(stages[currentStage + 1].cameraPosition, stages[currentStage + 1].cameraSize));
            StartStage(currentStage + 1);
        }
        else
        {
            Debug.Log("Level Complete!");
        }
    }

    private IEnumerator MoveCamera(Vector3 targetPosition, float targetSize)
    {
        Vector3 startPosition = mainCamera.transform.position;
        float startSize = mainCamera.orthographicSize;
        float elapsedTime = 0f;

        while (elapsedTime < cameraMoveSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / cameraMoveSpeed;

            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetSize, t);

            yield return null;
        }

        mainCamera.transform.position = targetPosition;
        mainCamera.orthographicSize = targetSize;
    }
}
