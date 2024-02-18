using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public enum EnemyType
{
    Chase,
    Static,
    Shoot,
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] Transform topLeftPoint;
    [SerializeField] Transform bottomRightPoint;
    [SerializeField] List<EnemyController> enemyPrefabs;
    [SerializeField] float delay;
    [SerializeField] int numberEnemyEachPool;
    [SerializeField] int spawnNumber;
    private PlayerController player;
    private int currentEnemyLoopCount;

    private List<EnemyPoolData> allPools = new();

    public void Init(PlayerController player)
    {
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            Queue<EnemyController> pools = new();
            PoolHelper.PoolObjects(out pools, enemyPrefabs[i].gameObject, numberEnemyEachPool);
            EnemyPoolData poolData = new(enemyPrefabs[i].EnemyType, pools);
            allPools.Add(poolData);
        }
        this.player = player;
        SpawnEnemy();
    }

    public Vector2 PickRandomPointInBound()
    {
        Vector2 randomPoint;
        randomPoint.x = Random.Range(topLeftPoint.position.x, bottomRightPoint.position.x);
        randomPoint.y = Random.Range(bottomRightPoint.position.y, topLeftPoint.position.y);
        return randomPoint;
    }

    public void DestroyEnemy(EnemyController enemy, float delayTime = 0f)
    {
        StartCoroutine(CorDestroyEnemy(enemy, delayTime));
    }

    private IEnumerator CorDestroyEnemy(EnemyController enemy, float delayTime = 0f)
    {
        yield return new WaitForSeconds(delay);
        enemy.gameObject.SetActive(false);
        PoolHelper.Destroy(GetMatchPool(enemy.EnemyType), enemy);
    }

    public void CheckSpawnEnemy()
    {
        StartCoroutine(CorCheckSpawnEnemy());
    }

    private IEnumerator CorCheckSpawnEnemy()
    {
        currentEnemyLoopCount++;
        if (currentEnemyLoopCount % spawnNumber == 0)
        {
            yield return new WaitForSeconds(2f);
            currentEnemyLoopCount = 0;
            SpawnEnemy();
            Debug.LogError("Spawn Enemy");
        }
    }

    public void SpawnEnemy()
    {
        for (int i = 0; i < spawnNumber; i++)
        {
            var enemy = PoolHelper.Spawn(GetRandomPool());
            enemy.gameObject.SetActive(true);
            enemy.StartSpawnEnemy(delay);
            enemy.SetPlayer(this.player.transform);
            enemy.transform.position = PickRandomPointInBound();
        }
    }

    public Queue<EnemyController> GetMatchPool(EnemyType poolType)
    {
        foreach (var pool in allPools)
        {
            if (pool.enemyPoolType == poolType)
                return pool.pool;
        }
        return (allPools != null && allPools.Count > 0) ? allPools[0].pool : null;
    }

    public Queue<EnemyController> GetRandomPool()
    {
        int random = Random.Range(0, allPools.Count);
        return allPools[random].pool;
    }
}

[System.Serializable]
public class EnemyPoolData
{
    public EnemyType enemyPoolType;
    public Queue<EnemyController> pool;

    public EnemyPoolData(EnemyType poolType, Queue<EnemyController> pool)
    {
        this.enemyPoolType = poolType;
        this.pool = pool;
    }
}
