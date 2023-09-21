using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class enemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefab;
    [SerializeField] float spawnInterval;
    [SerializeField] int maxEnemies;

    [SerializeField] int currentEnemyCount;
    [SerializeField] float spawnRadius;
    [SerializeField] GameObject shurikens;


    private float spawnTimer;
    void Update()
    {
            if (currentEnemyCount < maxEnemies)
            {
                spawnTimer -= Time.deltaTime;
                if (spawnTimer <= 0)
                {
                    spawnEnemy();
                    spawnTimer = spawnInterval;
                }
            }
    }
    void spawnEnemy()
    {
        int randomIndex = Random.Range(0, enemyPrefab.Length);
        GameObject selectedEnemyPrefab = enemyPrefab[randomIndex];

        Vector3 randomSpawnPosition = transform.position + Random.insideUnitSphere * spawnRadius; ;
        randomSpawnPosition.y = 0;

        GameObject enemy = Instantiate(selectedEnemyPrefab, randomSpawnPosition, Quaternion.identity);

        currentEnemyCount++;


    }
    public void EnemyDestroyed()
    {
        currentEnemyCount--;
    }
}
