using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 5f;
    public float spawnRadius = 5f;  // 生成半径设置为 10 米
    public float minSpawnDistance = 2f;  // 最小生成距离，避免敌人生成得太近
    public int maxEnemies = 5;  // 场上最大敌人数量
    public float enemyLifetime = 6f;  // 敌人生命周期

    private PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("PlayerController not found in the scene!");
            return;
        }
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (!GameManager.Instance.IsEasyMode && GameManager.Instance.isStarting && !player.isGameOver && GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        StartCoroutine(DestroyEnemyAfterDelay(enemy));
    }

        private IEnumerator DestroyEnemyAfterDelay(GameObject enemy)
    {
        yield return new WaitForSeconds(enemyLifetime);
        if (enemy != null)
        {
            Destroy(enemy);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPos;
        // 在 XZ 平面上生成随机位置
        Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minSpawnDistance, spawnRadius);
        randomPos = player.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        return randomPos;
    }
}