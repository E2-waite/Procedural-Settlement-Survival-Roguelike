using System.Collections.Generic;
using UnityEngine;
using static GridTile;

public class Chunk : MonoBehaviour
{
    public GameObject tilePrefab;
    public Vector2Int position;
    World world;
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colours;

    public float seaLevel = 0.35f;

    float[,] heights;
    public float heightMultiplier = 2.5f, heightScale = 1f;
    //public int size;

    ChunkResources resources;
    public Dictionary<Vector2Int, GridTile> tiles = new Dictionary<Vector2Int, GridTile>();

    Player player = null;
    public List<Agent> unitAgents = new List<Agent>();
    public List<Agent> enemyAgents = new List<Agent>();
    public  List<Chunk> neighbouringChunks = new List<Chunk>();
    private TileCatalog tileCatalog;
    bool empty = false;
    public bool IsEmpty => empty;
    public void AddNeighbour(Chunk chunk)
    {
        if (!neighbouringChunks.Contains(chunk))
        {
            neighbouringChunks.Add(chunk);
        }
    }

    public void Init(World world, Vector2Int pos, bool empty = false)
    {
        this.empty = empty;
        this.world = world;
        tileCatalog = world.Context.tileCatalog;
        int size = world.Context.chunkSize;

        // Chunks own their mesh, local tile lookup, agent lists, and resource renderer.
        WorldGrid grid = world.Context.grid;

        position = pos;

        mesh = new Mesh();

        vertices = new List<Vector3>();
        triangles = new List<int>();
        colours = new List<Color>();

        int vertexIndex = 0;

        heights = new float[size + 1, size + 1];

        // Heights need one extra row/column because each tile samples four corner vertices.
        for (int x = 0; x < size + 1; x++)
        {
            for (int y = 0; y < size + 1; y++)
            {
                if (empty)
                {
                    heights[x, y] = .4f;
                }
                else
                {
                    heights[x, y] = GetHeight(x + position.x * size, y + position.y * size, world.Context.noiseScale) - seaLevel;
                    if (heights[x, y] > 0) heights[x, y] *= heightMultiplier;
                    heights[x, y] = Mathf.Clamp01(heights[x, y]);
                }
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // Tile positions are stored in world grid coordinates, not chunk-local coordinates.
                Vector2Int tilePos = new Vector2Int(x + position.x * size, y + position.y * size);

                TileType tileType = GetTileType(x, y);
                GridTile tile = new GridTile(this, tileType, tilePos, new Vector3(tilePos.x, 0, tilePos.y));

                grid.SetTile(tile, tilePos);
                tiles[new Vector2Int(x, y)] = tile;
                AddTile(x, y, tile, ref vertexIndex);
            }
        }

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.SetColors(colours);

        mesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        if (!empty)
            resources = new ChunkResources(world, this);

        grid.SetChunk(this, pos);
    }

    public void AddAgent(Agent agent)
    {
        if (agent is Unit)
        {
            Unit follower = (Unit)agent;
            if (!unitAgents.Contains(follower))
                unitAgents.Add(follower);
        }
        else if (agent is Enemy)
        {
            Enemy enemy = (Enemy)agent;
            if (!enemyAgents.Contains(enemy))
                enemyAgents.Add(enemy);
        }
    }

    public void RemoveAgent(Agent agent)
    {
        if (agent is Unit)
        {
            Unit follower = (Unit)agent;
            if (unitAgents.Contains(follower))
                unitAgents.Remove(follower);
        }
        else if (agent is Enemy)
        {
            Enemy enemy = (Enemy)agent;
            if (enemyAgents.Contains(enemy))
                enemyAgents.Remove(enemy);
        }
    }

    public void AddPlayer(Player player)
    {
        this.player = player;
    }

    public void RemovePlayer()
    {
        player = null;
    }

    private void Update()
    {
        // Resources are drawn manually with instancing, so active chunks render them each frame.
        if (resources != null)
            resources.Render();
    }

    float GenerateNoise(float x, float y)
    {
        float value = 0f;
        float amplitude = 1f;
        float frequency = 1f;
        float maxValue = 0f;
 
        for (int i = 0; i < 4; i++)
        {
            float sampleX = x * frequency;
            float sampleY = y * frequency;

            value += Mathf.PerlinNoise(sampleX, sampleY) * amplitude;
            maxValue += amplitude;

            amplitude *= 0.5f;
            frequency *= 2f;
        }

        return value / maxValue;
    }

    float GetHeight(float x, float y, float noiseScale)
    {
        float height = GenerateNoise((x + world.Context.seedOffset.x) * noiseScale, (y + world.Context.seedOffset.y) * noiseScale);

        height = Mathf.Clamp01(height);
        height = Mathf.Pow(height, 1.2f);

        float step = 0.1f;
        height = Mathf.Round(height / step) * step;

        return height;
    }


    void AddTile(int x, int y, GridTile tile, ref int index)
    {
        // 4 corners of the tile
        vertices.Add(new Vector3(x, heights[x, y] * heightScale, y));
        vertices.Add(new Vector3(x, heights[x, y + 1] * heightScale, y + 1));
        vertices.Add(new Vector3(x + 1, heights[x + 1, y + 1] * heightScale, y + 1));
        vertices.Add(new Vector3(x + 1, heights[x + 1, y] * heightScale, y));

        Color tileColor = ColorFromType(tile.type);

        // same color for all 4 vertices
        colours.Add(tileColor);
        colours.Add(tileColor);
        colours.Add(tileColor);
        colours.Add(tileColor);

        // two triangles
        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 0);
        triangles.Add(index + 2);
        triangles.Add(index + 3);

        index += 4;
    }

    TileType GetTileType(int x, int y)
    {
        // A tile is water only if all four corners are below sea level.
        bool allWater = true;

        for (int ix = 0; ix < 2 && allWater; ix++)
        {
            for (int iy = 0; iy < 2 && allWater; iy++)
            {
                if (heights[x + ix, y + iy] > 0)
                {
                    allWater = false;
                }
            }
        }

        if (allWater)
        {
            return TileType.Water;
        }
        else
        {
            float height =
                (heights[x, y] +
                 heights[x + 1, y] +
                 heights[x, y + 1] +
                 heights[x + 1, y + 1]) * 0.25f;

            if (height < .25f)
            {
                return TileType.Sand;
            }
            else if (height < .5f)
            {
                return TileType.Grass;
            }
            else
            {
                return TileType.Forest;
            }
        }
    }


    Color ColorFromType(TileType tileType)
    {
        if (tileType == TileType.Water)
        {
            return tileCatalog.water.color;
        }
        else if (tileType == TileType.Sand)
        {
            return tileCatalog.sand.color;
        }
        else if (tileType == TileType.Grass)
        {
            return tileCatalog.grass.color;
        }
        else
        {
            return tileCatalog.forest.color;
        }
    }



    public List<Agent> GetEnemies(bool includeSurrounding = true)
    {
        if (!includeSurrounding) return enemyAgents;

        // Include neighbouring chunks so units near chunk edges can still detect each other.
        List<Agent> enemyList = new List<Agent>(enemyAgents);

        if (includeSurrounding)
        {
            foreach (Chunk neighbour in neighbouringChunks)
            {
                enemyList.AddRange(neighbour.GetEnemies(false));
            }
        }

        return enemyList;
    }

    public List<Agent> GetUnits(bool includeSurrounding = true)
    {
        if (!includeSurrounding) return unitAgents;

        List<Agent> followerList = new List<Agent>(unitAgents);

        if (includeSurrounding)
        {
            foreach (Chunk neighbour in neighbouringChunks)
            {
                followerList.AddRange(neighbour.GetUnits(false));
            }
        }

        return followerList;
    }
}
