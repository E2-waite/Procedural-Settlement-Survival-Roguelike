using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
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
    private bool hovering = false;

    public GridTile(Chunk inChunk, TileType type, Vector2Int pos, Vector3 worldPos)
    {
        chunk = inChunk;
        this.type = type;
        position = pos;
        worldPosition = worldPos;
    }

    public Vector3 Center()
    {
        return worldPosition + new Vector3(.5f, 0, .5f);
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

    public void SetHovering()
    {
        hovering = true;
    }

    public void ClearHovering()
    {
        hovering = false;
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
        //return type != TileType.Water && resource == ;
        return type != TileType.Water && building == null && resource == null;
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
