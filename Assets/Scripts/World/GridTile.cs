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

    public TileType type;
    public Chunk chunk= null;
    public Vector2Int position;
    public Vector3 worldPosition;

    Building building;
    ResourceNode resource = null;

    public GridTile(Chunk inChunk, TileType type, Vector2Int pos, Vector3 worldPos)
    {
        chunk = inChunk;
        this.type = type;
        position = pos;
        worldPosition = worldPos;
    }

    public void SetResource(ResourceNode node)
    {
        resource = node;
    }

    public ResourceNode Resource()
    {
        return resource;
    }

    public bool HasResource()
    {
        return resource != null;
    }

    public bool HasBuilding()
    {
        return building != null;
    }

    public Building Building()
    {
        return building;
    }

    public void Hover(bool active)
    {
       // Grid.Instance.HandleChunks(chunk.position);
    }

    public void Interact()
    {
        Debug.Log("Interacted with: " + type.ToString());
    }

    public bool IsEmpty()
    {
        return type != TileType.Water && building == null && resource == null;
    }

    // Check if this tile can be used in pathing
    public bool Walkable()
    {
        //return true;
        return type != TileType.Water;
        //return tileType != TileType.Water && building == null && resource == null;
    }
 
    public bool Buildable()
    {
        return type != TileType.Water && building == null && resource == null;
    }

    // Assign building/structure
    public void Build(Building build)
    {
        building = build;
    }
}
