using UnityEngine;

[System.Serializable]
public class WorldContext
{
    [Header("Prefabs")]
    public GameObject chunkPrefab;

    [Header("Params")]
    public Vector2Int startSize;
    public int chunkSize = 100;
    public int chunkDistance = 2;
    public float noiseScale = 0.05f;

    [HideInInspector] public WorldGrid grid;
    [HideInInspector] public Vector2 seedOffset;
}
