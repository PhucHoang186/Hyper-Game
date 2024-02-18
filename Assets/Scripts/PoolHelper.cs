using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolHelper : MonoBehaviour
{
    public static void PoolObjects<T>(out Queue<T> pool, GameObject objectToPool, int poolSize)
    {
        pool = new();
        for (int i = 0; i < poolSize; i++)
        {
            var prefab = objectToPool;
            var spawnObj = Instantiate(prefab);
            spawnObj.transform.position = Vector2.zero;
            spawnObj.SetActive(false);
            pool.Enqueue(spawnObj.GetComponent<T>());
        }
    }

    public static void Destroy<T>(Queue<T> pool, T objectToDestroy)
    {
        pool.Enqueue(objectToDestroy);
    }

    public static T Spawn<T>(Queue<T> pool)
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        return default(T);
    }
}
