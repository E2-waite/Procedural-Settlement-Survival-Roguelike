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
    public Vector3 Center => worldPosition + new Vector3(.5f, 0, .5f);
    public bool HasResource => resource != null && !resource.IsEmpty();
    public ResourceNode Resource => resource;

    public bool HasBuilding => building != null;
    public Building Building => building;
    public bool IsEmpty => type != TileType.Water && building == null && !HasResource;
    public bool Buildable => type != TileType.Water && building == null && resource == null;
    public void SetResource(ResourceNode node)
    {
        resource = node;
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

    // Check if this tile can be used in pathing

    // Assign building/structure
    public void Build(Building build)
    {
        building = build;
    }
}
