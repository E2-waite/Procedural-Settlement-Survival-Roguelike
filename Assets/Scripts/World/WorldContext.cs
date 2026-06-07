using UnityEngine;

[System.Serializable]
public class WorldContext
{
    [Header("Params")]
    public ResourceCatalog resourceCatalog;
    public TileCatalog tileCatalog;
    public Vector2Int startSize = new Vector2Int(3, 3);
    public float noiseScale = 0.05f;
    public GameObject chunkPrefab;
    public int chunkDistance = 2;
    public int chunkSize = 100;
    
    [HideInInspector] public WorldGrid grid;
    [HideInInspector] public Vector2 seedOffset;
}
