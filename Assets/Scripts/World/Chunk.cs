using System.Collections.Generic;
using UnityEngine;

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
    public List<Unit> followers = new List<Unit>();
    public List<Unit> enemies = new List<Unit>();
    public  List<Chunk> neighbouringChunks = new List<Chunk>();
    public void AddNeighbour(Chunk chunk)
    {
        if (!neighbouringChunks.Contains(chunk))
        {
            neighbouringChunks.Add(chunk);
        }
    }

    public void Init(World world, Vector2Int pos)
    {
        this.world = world;

        // Chunks own their mesh, local tile lookup, unit lists, and resource renderer.
        int size = world.Context.chunkSize;
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
                heights[x, y] = GetHeight(x + position.x * size, y + position.y * size, world.Context.noiseScale) - seaLevel;
                if (heights[x, y] > 0) heights[x, y] *= heightMultiplier;
                heights[x, y] = Mathf.Clamp01(heights[x, y]);

            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // Tile positions are stored in world grid coordinates, not chunk-local coordinates.
                Vector2Int tilePos = new Vector2Int(x + position.x * size, y + position.y * size);

                GridTile.TileType tileType = GetTileType(x, y);
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

        resources = new ChunkResources(world, this);

        grid.SetChunk(this, pos);
    }

    public void AddUnit(Unit unit)
    {
        if (unit is FollowerUnit)
        {
            FollowerUnit follower = (FollowerUnit)unit;
            if (!followers.Contains(follower))
                followers.Add(follower);
        }
        else if (unit is EnemyUnit)
        {
            EnemyUnit enemy = (EnemyUnit)unit;
            if (!enemies.Contains(enemy))
                enemies.Add(enemy);
        }
    }

    public void RemoveUnit(Unit unit)
    {
        if (unit is FollowerUnit)
        {
            FollowerUnit follower = (FollowerUnit)unit;
            if (followers.Contains(follower))
                followers.Remove(follower);
        }
        else if (unit is EnemyUnit)
        {
            EnemyUnit enemy = (EnemyUnit)unit;
            if (enemies.Contains(enemy))
                enemies.Remove(enemy);
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

    GridTile.TileType GetTileType(int x, int y)
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
            return GridTile.TileType.Water;
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
                return GridTile.TileType.Sand;
            }
            else if (height < .5f)
            {
                return GridTile.TileType.Grass;
            }
            else
            {
                return GridTile.TileType.Forest;
            }
        }
    }


    Color ColorFromType(GridTile.TileType tileType)
    {
        if (tileType == GridTile.TileType.Water)
        {
            return Color.blue;
        }
        else if (tileType == GridTile.TileType.Sand)
        {
            return Color.yellow;
        }
        else if (tileType == GridTile.TileType.Grass)
        {
            return Color.forestGreen;
        }
        else
        {
            return Color.darkGreen;
        }
    }



    public List<Unit> GetEnemies(bool includeSurrounding = true)
    {
        if (!includeSurrounding) return enemies;

        // Include neighbouring chunks so units near chunk edges can still detect each other.
        List<Unit> enemyList = new List<Unit>(enemies);

        if (includeSurrounding)
        {
            foreach (Chunk neighbour in neighbouringChunks)
            {
                enemyList.AddRange(neighbour.GetEnemies(false));
            }
        }

        return enemyList;
    }

    public List<Unit> GetFollowers(bool includeSurrounding = true)
    {
        if (!includeSurrounding) return followers;

        List<Unit> followerList = new List<Unit>(followers);

        if (includeSurrounding)
        {
            foreach (Chunk neighbour in neighbouringChunks)
            {
                followerList.AddRange(neighbour.GetFollowers(false));
            }
        }

        return followerList;
    }
}
