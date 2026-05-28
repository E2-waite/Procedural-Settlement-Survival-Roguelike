using UnityEngine;

public class ResourceNode
{
    public enum Density
    {
        Sparse = 0,
        Medium,
        Dense
    }

    public enum Type : int
    {
        Wood = 0,
        Stone,
        Max
    }

    public Type type;
    public Density density;

    public Vector3 worldPosition;
    public Vector2Int tilePosition;
    public ResourceObject resourceObj;
    public GridTile tile;
    public int index;

    int numLeft = 10;


    public ResourceNode(int i, Vector3 worldPos, Type type, GridTile tile, ResourceObject obj)
    {
        index = i;
        this.tile = tile;
        tilePosition = tile.position;
        worldPosition = worldPos;
        this.type = type;
        resourceObj = obj;
        tile.SetResource(this);
    }

    public bool IsEmpty()
    {
        return numLeft == 0;
    }

    public int Gather(int amount)
    {
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
