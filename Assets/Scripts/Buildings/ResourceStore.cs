using UnityEngine;

public class ResourceStore : Building
{
    public ResourceNode.Type type;

    public void Store(int count)
    {
        ResourceHandler.Instance.StoreResource(type, count);
    }

}
