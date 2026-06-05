using UnityEngine;

public class ResourceBuilding : Building
{
    public ResourceNode.Type type;
    ResourceSystem _resourceSystem;

    public void Init(ResourceSystem resourceSystem)
    {
        _resourceSystem = resourceSystem;
    }

    public void Store(int count)
    {
        _resourceSystem.StoreResource(type, count);
    }

}
