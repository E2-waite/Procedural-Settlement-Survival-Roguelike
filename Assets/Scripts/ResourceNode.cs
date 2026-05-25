using UnityEngine;

public class ResourceNode
{
    public enum Density
    {
        Sparse = 0,
        Medium,
        Dense
    }

    public enum Type
    {
        None,
        Tree,
        Stone
    }

    public Type resourceType;
    public Density density;

    public Vector3 worldPosition;
    public Vector2Int tilePosition;
    public ResourceObject resourceObj;
    public GridTile gridTile;
    public int index;

    int numLeft = 10;


    public ResourceNode(int i, Vector2Int pos, Vector3 worldPos, Type type, GridTile tile, ResourceObject obj)
    {
        index = i;
        gridTile = tile;
        tilePosition = pos;
        worldPosition = worldPos;
        resourceType = type;
        resourceObj = obj;
        tile.SetResource(this);
    }

    public bool IsEmpty()
    {
        return numLeft == 0;
    }

    public int Gather(int amount)
    {
        Debug.Log("GATHERING RESOURCE");
        int gathered = 0;
        if (amount > numLeft)
        {
            gathered = numLeft;
            numLeft = 0;
        }
        else
        {
            gathered = amount;
            numLeft -= amount;
        }

        // TODO: clear node from tile when empty

        return gathered;
    }
}
