using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private Dictionary<Vector2Int, Chunk> chunkGrid = new Dictionary<Vector2Int, Chunk>();
    private Dictionary<Vector2Int, GridTile> tileGrid = new Dictionary<Vector2Int, GridTile>();
    private WorldManager world;

    public Grid(WorldManager world)
    {
        this.world = world;
    }

    public void SetChunk(Vector2Int pos, Chunk chunk)
    {
        chunkGrid[pos] = chunk;
    }

    public bool HasChunk(Vector2Int pos)
    {
        return chunkGrid.ContainsKey(pos);
    }

    public Chunk GetChunk(Vector2Int pos)
    {
        Chunk chunk = null;
        if (chunkGrid.ContainsKey(pos))
            chunk = chunkGrid[pos];
        return chunk;
    }

    public void SetTile(Vector2Int pos, GridTile tile)
    {
        tileGrid[pos] = tile;
    }

    public bool HasTile(Vector2Int pos)
    {
        return tileGrid.ContainsKey(pos);
    }

    public GridTile GetTile(Vector2Int pos)
    {
        GridTile tile = null;
        if (tileGrid.ContainsKey(pos))
            tile = tileGrid[pos];
        return tile;
    }

    public GridTile GetTile(Vector3 pos)
    {
        return GetTile(new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z)));
    }

    public IEnumerable<GridTile> Tiles()
    {
        return tileGrid.Values;
    }

    public List<GridTile> GetNeighbours(Vector2Int pos)
    {
        List<GridTile> neighbours = new List<GridTile>();

        for (int x = pos.x - 1; x <= pos.x + 1; x++)
        {
            for (int y = pos.y - 1; y <= pos.y + 1; y++)
            {
                if (x == pos.x && y == pos.y)
                    continue;

                GridTile neighbouringTile = GetTile(new Vector2Int(x, y));
                if (neighbouringTile != null)
                    neighbours.Add(neighbouringTile);
            }
        }

        return neighbours;
    }

    public Chunk ChunkFromGridPos(Vector2Int pos)
    {
        Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt((float)pos.x / world.chunkSize), Mathf.FloorToInt((float)pos.y / world.chunkSize));

        return GetChunk(chunkPos);
    }
}
