using Mono.Cecil;
using UnityEngine;

[System.Serializable]
public class ResourceStorage
{
    public int[] resourceCount = new int[(int)ResourceNode.Type.Max];
    public int max;

    public void Add(ResourceNode.Type type, int count)
    {
        resourceCount[(int)type] += count;
    }

    public void Remove(ResourceNode.Type type, int count)
    {
        resourceCount[(int)type] -= count;
    }

    public bool IsEmpty(ResourceNode.Type type)
    {
        return resourceCount[(int)type] <= 0;
    }

    public void Clear(ResourceNode.Type type)
    {
        resourceCount[(int)type] = 0;
    }

    public int Get(ResourceNode.Type type)
    {
        return resourceCount[(int)type];
    }

    public bool AtCapacity()
    {
        return Total() >= max;
    }

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
