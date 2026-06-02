using UnityEngine;

public class ResourceBuilding : Building
{
    public ResourceNode.Type type;

    public void Store(int count)
    {
        ResourceSystem.Instance.StoreResource(type, count);
    }

}
