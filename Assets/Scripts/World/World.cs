using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] WorldContext context = new();
    public WorldContext Context => context;
    ChunkStreaming chunkStreaming;

    // Generates the initial chunks
    public void Generate(GameContext gameContext)
    {
        chunkStreaming = gameContext.chunkStreaming;

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
                chunk.Init(this, chunkPos);
                chunkObj.transform.parent = transform;
                chunk.name = "Chunk: " + chunkPos.ToString();
            }
        }
    }

    public void GenerateEmpty(GameContext gameContext)
    {
        chunkStreaming = gameContext.chunkStreaming;
        context.grid = new WorldGrid(context);

        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                GameObject chunkObj = Instantiate(context.chunkPrefab, new Vector3(chunkPos.x * context.chunkSize, 0, chunkPos.y * context.chunkSize), Quaternion.identity);
                Chunk chunk = chunkObj.GetComponent<Chunk>();
                chunk.Init(this, chunkPos, true);
                chunkObj.transform.parent = transform;
                chunk.name = "Chunk: " + chunkPos.ToString();
            }
        }
    }

    public void GenerateEmpty()
    {

    }

    public void InitStartChunks()
    {
        for (int x = -1; x < 1; x++)
        {
            for (int y = -1; y < 1; y++)
            {
                Vector2Int chunkPos = new Vector2Int(x, y);
                if (context.grid.HasChunk(chunkPos))
                {
                    Chunk chunk = context.grid.GetChunk(chunkPos);
                    chunkStreaming.UpdateChunkNeighbours(chunk);
                }
            }
        }
    }

    private Vector2 GenerateSeedOffset(int seed)
    {
        System.Random rand = new System.Random(seed);

        float x = (float)(rand.NextDouble() * 20000 - 10000);
        float y = (float)(rand.NextDouble() * 20000 - 10000);

        return new Vector2(x, y);
    }
}
