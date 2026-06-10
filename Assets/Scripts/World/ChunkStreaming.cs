using System.Collections.Generic;
using UnityEngine;

public class ChunkStreaming : MonoBehaviour
{

    private HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();
    private HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();
    private Chunk lastChunk;
    private WorldGrid grid;
    private World world;
    private GameObject chunkPrefab;
    private int chunkDistance;
    private int chunkSize;

    public void Init(World world)
    {
        this.world = world;


        grid = world.Context.grid;
        chunkDistance = world.Context.chunkDistance;
        chunkSize = world.Context.chunkSize;
        chunkPrefab = world.Context.chunkPrefab;
    }

    // Updates chunk's neighbours
    public void UpdateChunkNeighbours(Chunk chunk)
    {
        // Neighbour links let unit queries include nearby chunks without scanning the whole world.
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
    public void UpdateChunks(Chunk newChunk)
    {
        if (lastChunk == newChunk) return; // Don't handle chunks if we've already handled this pos
        lastChunk = newChunk;

        requiredChunks.Clear();

        // Build the active window around the current chunk.
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
                CreateChunk(chunkPos, newChunk.IsEmpty);
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

    void CreateChunk(Vector2Int pos, bool emptyChunk)
    {
        // Create a new chunk if there isn't one
        GameObject chunkObj = Instantiate(chunkPrefab, new Vector3(pos.x * chunkSize, 0, pos.y * chunkSize), Quaternion.identity);
        chunkObj.transform.parent = transform;
        Chunk chunk = chunkObj.GetComponent<Chunk>();
        chunk.Init(world, pos, emptyChunk);
        chunk.name = "Chunk: " + pos.ToString();
        grid.SetChunk(chunk, pos);
        UpdateChunkNeighbours(chunk);
    }
}
