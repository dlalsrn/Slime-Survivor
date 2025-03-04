using System.Collections;
using System.Security;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy[] enemyPrefabs;
    [SerializeField] private Enemy bossPrefab;
    [SerializeField] private Transform[] spawnPoints;

    private int spawnTimes = 5; // 적 Spawn을 반복하는 횟수
    private int spawnEnemies = 10; // 한 번에 Spawn하는 Enemy의 개수
    private float spawnInterval = 5f;

    private Coroutine enemySpawnCoroutine;

    public void StartEnemySpawn() {
        enemySpawnCoroutine = StartCoroutine(EnemySpawnRoutine());
    }

    public void StopEnemySpawn() {
        StopCoroutine(enemySpawnCoroutine);
    }

    IEnumerator EnemySpawnRoutine() {
        yield return new WaitForSeconds(1f);
        for (int i = 0; i < spawnTimes; i++) {
            for (int j = 0; j < spawnEnemies; j++) {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
        SpawnBoss();
    }

    private void SpawnEnemy() {
        int enemyIndex = Random.Range(0, enemyPrefabs.Length); // 적의 종류
        int pointIndex = Random.Range(0, spawnPoints.Length); // Spawn하는 위치
        Instantiate<Enemy>(enemyPrefabs[enemyIndex], spawnPoints[pointIndex].position, Quaternion.identity); 
    }

    private void SpawnBoss() {
        int pointIndex = Random.Range(0, spawnPoints.Length); // Spawn하는 위치
        Instantiate<Enemy>(bossPrefab, spawnPoints[pointIndex].position, Quaternion.identity); 
    }
}
