using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] WorldContext context = new();
    public WorldContext Context => context;

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
        // The seed offset keeps terrain deterministic after generation while varying each new run.
        context.seedOffset = GenerateSeedOffset(System.DateTime.Now.Ticks.GetHashCode());
        context.grid = new WorldGrid(context);

        // Generate the initial grid
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                GameObject chunkObj = Instantiate(context.chunkPrefab, new Vector3(chunkPos.x * context.chunkSize, 0, chunkPos.y * context.chunkSize), Quaternion.identity);
                Chunk chunk = chunkObj.GetComponent<Chunk>();
                chunk.Init(context);
                chunk.Generate(context, chunkPos);
                chunkObj.transform.parent = transform;
                chunk.name = "Chunk: " + chunkPos.ToString();
            }
        }


        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                if (context.grid.HasChunk(chunkPos))
                {
                    Chunk chunk = context.grid.GetChunk(chunkPos);
                    UpdateChunkNeighbours(chunk);
                }
            }
        }
    }

    // Updates chunk's neighbours
    void UpdateChunkNeighbours(Chunk chunk)
    {
        // Neighbour links let unit queries include nearby chunks without scanning the whole world.
        for (int i = 0; i < Consts.ALL_NEIGHBOURS.Length; i++)
        {
            Vector2Int neighbourPos = chunk.position + Consts.ALL_NEIGHBOURS[i];

            if (context.grid.HasChunk(neighbourPos))
            {
                Chunk neighbour = context.grid.GetChunk(neighbourPos);
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

        // Build the active window around the current chunk.
        for (int x = newChunk.position.x - 2; x <= newChunk.position.x + context.chunkDistance; x++)
        {
            for (int y = newChunk.position.y - 2; y <= newChunk.position.y + context.chunkDistance; y++)
            {
                requiredChunks.Add(new Vector2Int(x, y));
            }
        }

        // Activate/create required chunks
        foreach (Vector2Int chunkPos in requiredChunks)
        {
            if (context.grid.HasChunk(chunkPos))
            {
                ActivateChunk(true, context.grid.GetChunk(chunkPos));
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
                if (context.grid.HasChunk(chunkPos))
                {
                    ActivateChunk(false, context.grid.GetChunk(chunkPos));
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
        GameObject chunkObj = Instantiate(context.chunkPrefab, new Vector3(pos.x * context.chunkSize, 0, pos.y * context.chunkSize), Quaternion.identity);
        chunkObj.transform.parent = transform;
        Chunk chunk = chunkObj.GetComponent<Chunk>();
        chunk.Init(context);
        chunk.Generate(context, pos);
        chunk.name = "Chunk: " + pos.ToString();
        context.grid.SetChunk(chunk, pos);
        UpdateChunkNeighbours(chunk);
    }

    public void SpawnBuilding(BuildingObject building)
    {

    }
}
