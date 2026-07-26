using System.Collections.Generic;
using UnityEngine;
using static GridTile;

//[ExecuteAlways]
public class Chunk : MonoBehaviour
{
    public Vector2Int position;
    private ChunkData data = new();
    public ChunkData Data => data;

    WorldBuilder world;
    Mesh mesh;
    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colours;

    public float seaLevel = 0.35f;

    float[,] heights;
    //public int size;

    ChunkResources resources;
    public Dictionary<Vector2Int, GridTile> tiles = new Dictionary<Vector2Int, GridTile>();

    private TileCatalog tileCatalog;
    bool empty = false;
    private float step = .1f;
    private bool smooth = false;
    public bool IsEmpty => empty;


    public void Init(WorldBuilder world, Vector2Int pos, bool empty = false)
    {
        this.empty = empty;
        this.world = world;
        tileCatalog = world.Context.tileCatalog;
        int size = world.Context.chunkSize;
        smooth = world.Context.smooth;
        step = world.Context.step;
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
                    if (heights[x, y] > 0) heights[x, y] *= world.Context.heightMultiplier;
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

        //float step = 0.1f;
        //height = Mathf.Round(height / step) * step;

        return height;
    }

    float StepHeight(float height)
    {
        return Mathf.Round(height / step) * step;
    }

    void AddTile(int x, int y, GridTile tile, ref int index)
    {
        if (smooth)
        {
            vertices.Add(new Vector3(x, heights[x, y] * world.Context.heightScale, y));
            vertices.Add(new Vector3(x, heights[x, y + 1] * world.Context.heightScale, y + 1));
            vertices.Add(new Vector3(x + 1, heights[x + 1, y + 1] * world.Context.heightScale, y + 1));
            vertices.Add(new Vector3(x + 1, heights[x + 1, y] * world.Context.heightScale, y));
        }
        else
        {
            float tileHeight = HeightFromType(tile.type) * world.Context.heightScale;

            vertices.Add(new Vector3(x, tileHeight, y));
            vertices.Add(new Vector3(x, tileHeight, y + 1));
            vertices.Add(new Vector3(x + 1, tileHeight, y + 1));
            vertices.Add(new Vector3(x + 1, tileHeight, y));
        }


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

    float HeightFromType(TileType tileType)
    {
        if (tileType == TileType.Water)
        {
            return 0f;
        }
        else if (tileType == TileType.Sand)
        {
            return .2f;
        }
        else
        {
            return .4f;
        }
    }


}
