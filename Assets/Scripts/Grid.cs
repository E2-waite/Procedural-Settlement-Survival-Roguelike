using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Grid : MonoSingleton<Grid>
{
    public Vector2Int size;
    public int chunkSize = 100;
    public int chunkDistance = 2;
    public float noiseScale = 0.5f;
    public GameObject chunkPrefab;
    private Dictionary<Vector2Int, Chunk> chunkGrid = new Dictionary<Vector2Int, Chunk>();
    private Dictionary<Vector2Int, GridTile> tileGrid = new Dictionary<Vector2Int, GridTile>();

    private HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();
    private Vector2Int lastChunkPos;

    public Vector2 seedOffset;

    void Start()
    {
        seedOffset = GenerateSeedOffset(System.DateTime.Now.Ticks.GetHashCode()); 
        GenerateGrid();
    }

    Vector2 GenerateSeedOffset(int seed)
    {
        System.Random rand = new System.Random(seed);

        float x = (float)(rand.NextDouble() * 20000 - 10000);
        float y = (float)(rand.NextDouble() * 20000 - 10000);

        return new Vector2(x, y);
    }

    void GenerateGrid()
    {
        // Generate the initial grid
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                GameObject chunkObj = Instantiate(chunkPrefab, new Vector3(chunkPos.x * chunkSize, 0, chunkPos.y * chunkSize), Quaternion.identity);
                Chunk chunk = chunkObj.GetComponent<Chunk>();
                chunk.Generate(this, chunkPos, chunkSize, noiseScale);
                chunkObj.transform.parent = transform;
                chunkGrid[chunkPos] = chunk;
                
            }
        }
    }

    // Handle enabling/creation and disabling of valid/invalid chunks
    public void HandleChunks(Vector2Int pos)
    {
        if (lastChunkPos == pos) return; // Don't handle chunks if we've already handled this pos
        lastChunkPos = pos;

        requiredChunks.Clear();

        // Get the chunk positions we want to be active (5 x 5 grid)
        for (int x = pos.x - 2; x <= pos.x + chunkDistance; x++)
        {
            for (int y = pos.y - 2; y <= pos.y + chunkDistance; y++)
            {
                requiredChunks.Add(new Vector2Int(x, y));
            }
        }

        // Activate/create required chunks
        foreach (Vector2Int chunkPos in requiredChunks)
        {
            Chunk chunk = getChunk(chunkPos);
        }

        // Disable non-required chunks
        foreach (var chunkPos in activeChunks)
        {
            if (!requiredChunks.Contains(chunkPos))
            {
                Chunk chunk = chunkGrid[chunkPos];
                if (chunk != null && chunk.gameObject.activeSelf)
                    chunk.gameObject.SetActive(false);
            }
        }

        activeChunks.Clear();
        activeChunks.UnionWith(requiredChunks);
    }

    public Chunk getChunk(Vector2Int pos)
    {
        Chunk chunk;
        if (chunkGrid.ContainsKey(pos))
        {
            // Return the chunk if there is one
            chunk = chunkGrid[pos];
            if (!chunk.gameObject.activeSelf)
                chunk.gameObject.SetActive(true);
        }
        else
        {
            // Create a new chunk if there isn't one
            GameObject chunkObj = Instantiate(chunkPrefab, new Vector3(pos.x * chunkSize, 0, pos.y * chunkSize), Quaternion.identity);
            chunkObj.transform.parent = transform;
            chunk = chunkObj.GetComponent<Chunk>();
            chunk.Generate(this, pos, chunkSize, noiseScale);
            chunkGrid[pos] = chunk;
        }

        return chunk;
    }

    public void setTile(Vector2Int pos, GridTile tile)
    {
        tileGrid[pos] = tile;
    }

    public GridTile getTile(Vector2Int pos)
    {
        if (tileGrid.ContainsKey(pos))
        {
            // Return the tile if there is one
            return tileGrid[pos];
        }
        return null;
    }

    Vector2Int gridPos = new Vector2Int(), lastGridPos = new Vector2Int();
    GridTile lastTile = null;
    public GridTile getTile(Vector3 worldPos)
    {
        Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.z));

        GridTile tile = getTile(gridPos);

        return tile;

    }
}
