using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.LightTransport;
using static GridTile;

public class Chunk : MonoBehaviour
{
    public GameObject tilePrefab;
    public Vector2Int position;
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colours;

    public float seaLevel = 0.35f;

    float[,] heights;
    public float heightMultiplier = 2.5f, heightScale = 1f;
    public int size;

    ChunkResources resources;
    private Dictionary<Vector2Int, GridTile> tiles = new Dictionary<Vector2Int, GridTile>();

    public List<Unit> followers = new List<Unit>();
    public List<EnemyUnit> enemies = new List<EnemyUnit>();
   public  List<Chunk> neighbouringChunks = new List<Chunk>();

    public void AddNeighbour(Chunk chunk)
    {
        if (!neighbouringChunks.Contains(chunk))
        {
            neighbouringChunks.Add(chunk);
        }
    }

    // Generate this chunk's mesh 
    public void Generate(Grid theGrid, Vector2Int pos, int chunkSize, float noiseScale)
    {
        size = chunkSize;
        position = pos;

        mesh = new Mesh();

        vertices = new List<Vector3>();
        triangles = new List<int>();
        colours = new List<Color>();

        int vertexIndex = 0;

        heights = new float[chunkSize + 1, chunkSize + 1];

        for (int x = 0; x < chunkSize + 1; x++)
        {
            for (int y = 0; y < chunkSize + 1; y++)
            {
                heights[x, y] = GetHeight(x + position.x * chunkSize, y + position.y * chunkSize, noiseScale) - seaLevel;
                if (heights[x, y] > 0) heights[x, y] *= heightMultiplier;
                heights[x, y] = Mathf.Clamp01(heights[x, y]);

            }
        }

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2Int tilePos = new Vector2Int(x + position.x * chunkSize, y + position.y * chunkSize);

                GridTile.TileType tileType = GetTileType(x, y);
                GridTile tile = new GridTile(this, tileType, tilePos, new Vector3(tilePos.x, 0, tilePos.y));

                Grid.Instance.setTile(tilePos, tile);
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

        // TODO: assign neighbouring chunks

        resources = new ChunkResources(this);
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

    private void Update()
    {
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
        float height = GenerateNoise((x + Grid.Instance.seedOffset.x) * noiseScale, (y + Grid.Instance.seedOffset.y) * noiseScale);

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

        Color tileColor = ColorFromType(tile.tileType);

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
}
