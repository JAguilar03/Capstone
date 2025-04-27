using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

// This script manages the spawning of enemies in the game. It randomly selects an enemy type based on spawn weights 
// and spawns them at various spawn points. The script ensures that the total number of enemies does not exceed a 
// specified limit and that enemies spawn outside the camera view or too close to the camera. It also handles enemy 
// removal and updates the enemy count when an enemy dies.

[Serializable]
// This class represents an enemy type with its associated prefab, spawn weight, maximum count, and current count.
public class EnemyType
{
    public GameObject enemyPrefab;
    public float spawnWeight = 1f; // Higher weight = more likely to spawn
    public int maxCount = 5; // Maximum number of this type
    [HideInInspector]
    public int currentCount = 0;
}

// This class manages the spawning of enemies in the game. It randomly selects an enemy type based on spawn weights
public class EnemySpawner : MonoBehaviour
{
    public EnemyType[] enemyTypes;
    public List<Transform> spawnPoints;
    public float spawnInterval = 5f;
    public int maxTotalEnemies = 10;
    public float minSpawnDistanceFromCamera = 10f;

    private int currentTotalEnemies = 0;
    private Camera mainCamera;

    public bool isBSP = false;

    // This method is called when the script is initialized. It initializes the main camera and starts the coroutine for spawning enemies.
    void Start()
    {
        mainCamera = Camera.main;
        if (!isBSP) {
            StartCoroutine(SpawnEnemies());
        }
    }

    void LateUpdate() 
    {
        // If BSP is enabled, add spawn points from GameObjects with the "EnemySpawn" tag
        if (isBSP) {
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("EnemySpawn")) {
                spawnPoints.Add(go.transform);
            }
            StartCoroutine(SpawnEnemies());
            isBSP = false;
        }
    }


    // This coroutine spawns enemies at random spawn points at regular intervals.
    bool IsSpawnPointVisible(Vector3 spawnPosition)
    {
        Vector3 viewportPoint = mainCamera.WorldToViewportPoint(spawnPosition);
        bool inCameraView = viewportPoint.x >= 0 && viewportPoint.x <= 1 && 
                           viewportPoint.y >= 0 && viewportPoint.y <= 1 && 
                           viewportPoint.z > 0;
                           
        float distanceToCamera = Vector3.Distance(spawnPosition, mainCamera.transform.position);
        
        return inCameraView || distanceToCamera < minSpawnDistanceFromCamera;
    }

    // This method selects a random enemy type based on spawn weights.
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

    // This coroutine spawns enemies at random spawn points at regular intervals.
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
                        int randomSpawnPoint = UnityEngine.Random.Range(0, spawnPoints.Count);
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

    // This method is called when an enemy dies. It decrements the total enemy count and updates the enemy count for the specific enemy type.
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
