using System.Collections.Generic;
using UnityEngine;

// Class for storing pools of objects to be reused instead of creating new ones
public class ObjectPool<T> where T : Component
{
    Queue<T> pool = new();
    private T prefab;
    private int poolSize = 0;
    private int instances = 0;
    Transform parent;

    public ObjectPool(T prefab, int poolSize, Transform parent)
    {
        this.prefab = prefab;
        this.poolSize = poolSize;
        this.parent = parent;
    }

    // Pre-warm the pool by creating the specified number of objects
    public void PreWarm()
    {
        for (int i = 0; i < poolSize; i++)
        {
            T obj = Create(false);
            if (obj == null)
            {
                Debug.LogWarning($"Failed to create object of type {typeof(T).Name} for the pool.");
                continue;
            }
            pool.Enqueue(obj);
        }
    }

    // Create a new object and add it to the pool
    private T Create(bool active)
    {
        if (prefab == null || instances >= poolSize) return null;

        T obj = Object.Instantiate(prefab, parent);
        obj.gameObject.SetActive(active);
        instances++;

        return obj;
    }

    // Get an object from the pool, if the pool is empty, create a new one
    public T Get()
    {
        if (pool.Count == 0) return Create(true);

        T obj = pool.Dequeue();
        obj.gameObject.SetActive(true);

        return obj;
    }

    public void Return(T obj)
    {
        if (obj == null) return;
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}