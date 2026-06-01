using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoSingleton<WorldManager>
{
    public GameObject chunkPrefab;
    public Vector2Int size;
    public int chunkSize = 100;
    public int chunkDistance = 2;
    public float noiseScale = 0.05f;
    public Vector2 seedOffset;
    public static Grid grid;
    private HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();
    private Chunk lastChunk;

    Vector2 GenerateSeedOffset(int seed)
    {
        System.Random rand = new System.Random(seed);

        float x = (float)(rand.NextDouble() * 20000 - 10000);
        float y = (float)(rand.NextDouble() * 20000 - 10000);

        return new Vector2(x, y);
    }

    // Generates the initial chunks
    public void GenerateGrid()
    {
        seedOffset = GenerateSeedOffset(System.DateTime.Now.Ticks.GetHashCode());
        grid = new Grid(this);

        // Generate the initial grid
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                GameObject chunkObj = Instantiate(chunkPrefab, new Vector3(chunkPos.x * chunkSize, 0, chunkPos.y * chunkSize), Quaternion.identity);
                Chunk chunk = chunkObj.GetComponent<Chunk>();
                chunk.Generate(grid, chunkPos, chunkSize, noiseScale);
                chunkObj.transform.parent = transform;
                grid.SetChunk(chunkPos, chunk);
                chunk.name = "Chunk: " + chunkPos.ToString();
            }
        }


        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                if (grid.HasChunk(chunkPos))
                {
                    Chunk chunk = grid.GetChunk(chunkPos);
                    UpdateChunkNeighbours(chunk);
                }
            }
        }
    }

    // Updates chunk's neighbours
    void UpdateChunkNeighbours(Chunk chunk)
    {
        for (int i = 0; i < Consts.ALL_NEIGHBOURS.Length; i++)
        {
            Vector2Int neighbourPos = chunk.position + Consts.ALL_NEIGHBOURS[i];

            if (grid.HasChunk(neighbourPos))
            {
                Chunk neighbour = grid.GetChunk(neighbourPos);
                neighbour.AddNeighbour(chunk);
                chunk.AddNeighbour(neighbour);
            }
        }
    }


    // Handle enabling/creation and disabling of valid/invalid chunks
    public void HandleChunks(Chunk newChunk)
    {
        if (lastChunk == newChunk) return; // Don't handle chunks if we've already handled this pos
        lastChunk = newChunk;

        requiredChunks.Clear();

        // Get the chunk positions we want to be active (5 x 5 grid)
        for (int x = newChunk.position.x - 2; x <= newChunk.position.x + chunkDistance; x++)
        {
            for (int y = newChunk.position.y - 2; y <= newChunk.position.y + chunkDistance; y++)
            {
                requiredChunks.Add(new Vector2Int(x, y));
            }
        }

        // Activate/create required chunks
        foreach (Vector2Int chunkPos in requiredChunks)
        {
            if (grid.HasChunk(chunkPos))
            {
                ActivateChunk(true, grid.GetChunk(chunkPos));
            }
            else
            {
                CreateChunk(chunkPos);
            }
        }

        // Disable non-required chunks
        foreach (var chunkPos in activeChunks)
        {
            if (!requiredChunks.Contains(chunkPos))
            {
                if (grid.HasChunk(chunkPos))
                {
                    ActivateChunk(false, grid.GetChunk(chunkPos));
                }
            }
        }

        activeChunks.Clear();
        activeChunks.UnionWith(requiredChunks);
    }

    void ActivateChunk(bool active, Chunk chunk)
    {
        if (chunk != null && chunk.gameObject.activeSelf != active)
        {
            chunk.gameObject.SetActive(active);
        }
    }

    void CreateChunk(Vector2Int pos)
    {
        // Create a new chunk if there isn't one
        GameObject chunkObj = Instantiate(chunkPrefab, new Vector3(pos.x * chunkSize, 0, pos.y * chunkSize), Quaternion.identity);
        chunkObj.transform.parent = transform;
        Chunk chunk = chunkObj.GetComponent<Chunk>();
        chunk.Generate(grid, pos, chunkSize, noiseScale);
        chunk.name = "Chunk: " + pos.ToString();
        grid.SetChunk(pos, chunk);
        UpdateChunkNeighbours(chunk);
    }

    public void SpawnBuilding(BuildingObject building)
    {

    }
}
