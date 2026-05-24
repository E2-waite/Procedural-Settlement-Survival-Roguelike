using UnityEngine;
using UnityEngine.Rendering;

public class GridTile
{
    public enum TileType
    {
        Water,
        Sand,
        Grass,
        Forest,
        Mountain
    }

    public TileType tileType;
    public Chunk chunk= null;
    public Vector2Int position;

    public int gCost;
    public int hCost;

    public int fCost => gCost + hCost;

    GameObject building;
    ResourceNode resource = null;

    public GridTile(Chunk inChunk, TileType type, Vector2Int pos)
    {
        chunk = inChunk;
        tileType = type;
        position = pos;
    }

    public void SetResource(ResourceNode node)
    {
        resource = node;
    }

    public ResourceNode GetResource()
    {
        return resource;
    }

    public void Hover(bool active)
    {
       // Grid.Instance.HandleChunks(chunk.position);
    }

    public void Interact()
    {
        Debug.Log("Interacted with: " + tileType.ToString());
    }

    // Check if this tile can be used in pathing
    public bool Walkable()
    {
        //return true;
        return tileType != TileType.Water && building == null && resource == null;
    }
 
    public bool Buildable()
    {
        return tileType != TileType.Water && building == null && resource == null;
    }

    // Assign building/structure
    public void Build(GameObject build)
    {
        building = build;
    }
}
