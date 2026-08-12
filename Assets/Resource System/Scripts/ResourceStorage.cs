[System.Serializable]
public class ResourceStorage
{
    public int[] resourceCount = new int[(int)ResourceNode.Type.Max];
    public int max;

    // Adds resources of type to the storage
    public void Add(ResourceNode.Type type, int count)
    {
        resourceCount[(int)type] += count;
    }

    // Removes resources of type from storage 
    public void Remove(ResourceNode.Type type, int count)
    {
        resourceCount[(int)type] -= count;
    }

    // Returns the current resource count of type
    public int Get(ResourceNode.Type type)
    {
        return resourceCount[(int)type];
    }

    // Clears all resources of type
    public void Clear(ResourceNode.Type type)
    {
        resourceCount[(int)type] = 0;
    }

    // Returns true if no resources are stored of type
    public bool IsEmpty(ResourceNode.Type type)
    {
        return resourceCount[(int)type] <= 0;
    }

    // Returns true if the total number of resources is at the max
    public bool AtCapacity()
    {
        return Total() >= max;
    }

    // Returns the total number of resources across all types
    public int Total()
    {
        int total = 0;
        for (int i = 0; i < (int)ResourceNode.Type.Max; i++)
        {
            total += resourceCount[i];
        }
        return total;
    }
}
