using System.Collections.Generic;
using UnityEngine;

// Class for storing pools of objects to be reused instead of creating new ones
public class ObjectPool<T> where T : Component, IPoolable
{
    Queue<T> pool = new();
    private T prefab;
    private int poolSize = 0;
    private int instances = 0;
    private Transform parent;
    private Vector3 offset;
    public Vector3 Offset => offset;
    public ObjectPool(T prefab, int poolSize, Transform parent, Vector3 offset)
    {
        this.prefab = prefab;
        this.poolSize = poolSize;
        this.parent = parent;
        this.offset = offset;
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
        obj.Init();
        obj.gameObject.SetActive(active);
        instances++;

        return obj;
    }

    // Get an object from the pool, if the pool is empty, create a new one
    public T Get()
    {
        T obj = pool.Count == 0 ? Create(true) : pool.Dequeue();
        if (obj == null) return null;
        obj.OnGet();
        return obj;
    }

    public void Return(T obj)
    {
        if (obj == null) return;
        obj.OnReturn();
        pool.Enqueue(obj);
    }
}