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

    public int TypeInt => (int)type;
    public Type type;
    public Density density;
    public ResourceObject resourceObj;
    public GridTile tile;
    public int index;

    public int remaining = 10;


    public ResourceNode(int i, Vector3 worldPos, Type type, GridTile tile, ResourceObject obj)
    {
        index = i;
        this.tile = tile;
        this.type = type;
        resourceObj = obj;
        tile.SetResource(this);
    }

    public bool IsEmpty()
    {
        return remaining == 0;
    }

    public int Gather(int amount)
    {
        int gathered = 0;
        if (amount > remaining)
        {
            gathered = remaining;
            remaining = 0;
        }
        else
        {
            gathered = amount;
            remaining -= amount;
        }

        // TODO: clear node from tile when empty

        return gathered;
    }
}
