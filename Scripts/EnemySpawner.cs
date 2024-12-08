using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class EnemyType
{
    public GameObject enemyPrefab;
    public float spawnWeight = 1f; // Higher weight = more likely to spawn
    public int maxCount = 5; // Maximum number of this type
    [HideInInspector]
    public int currentCount = 0;
}

public class EnemySpawner : MonoBehaviour
{
    public EnemyType[] enemyTypes;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int maxTotalEnemies = 10;
    public float minSpawnDistanceFromCamera = 10f;

    private int currentTotalEnemies = 0;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        StartCoroutine(SpawnEnemies());
    }

    bool IsSpawnPointVisible(Vector3 spawnPosition)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(spawnPosition);
        bool inCameraView = viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
                           viewportPoint.y >= 0 && viewportPoint.y <= 1 && 
                           viewportPoint.z > 0;
                           
        float distanceToCamera = Vector3.Distance(spawnPosition, mainCamera.transform.position);
        
        return inCameraView || distanceToCamera < minSpawnDistanceFromCamera;
    }

    EnemyType SelectRandomEnemyType()
    {
        float totalWeight = 0;
        foreach (var enemyType in enemyTypes)
        {
            if (enemyType.currentCount < enemyType.maxCount)
                totalWeight += enemyType.spawnWeight;
        }

        float random = UnityEngine.Random.Range(0, totalWeight);
        float currentWeight = 0;

        foreach (var enemyType in enemyTypes)
        {
            if (enemyType.currentCount < enemyType.maxCount)
            {
                currentWeight += enemyType.spawnWeight;
                if (random <= currentWeight)
                    return enemyType;
            }
        }

        return null;
    }

    IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (currentTotalEnemies < maxTotalEnemies)
            {
                EnemyType selectedEnemyType = SelectRandomEnemyType();
                
                if (selectedEnemyType != null)
                {
                    for (int attempts = 0; attempts < 10; attempts++)
                    {
                        int randomSpawnPoint = UnityEngine.Random.Range(0, spawnPoints.Length);
                        Vector3 spawnPosition = spawnPoints[randomSpawnPoint].position;

                        if (!IsSpawnPointVisible(spawnPosition))
                        {
                            GameObject newEnemy = Instantiate(selectedEnemyType.enemyPrefab, spawnPosition, Quaternion.identity);
                            selectedEnemyType.currentCount++;
                            currentTotalEnemies++;
                            break;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public void OnEnemyDeath(GameObject enemyObject)
    {
        currentTotalEnemies--;
        
        // Find and update the count for the specific enemy type
        foreach (var enemyType in enemyTypes)
        {
            if (enemyObject.CompareTag(enemyType.enemyPrefab.tag))
            {
                enemyType.currentCount--;
                break;
            }
        }
    }
}
