using UnityEngine;

public class ResourceBuilding : Building
{
    public ResourceNode.Type type;
    ResourceSystem resourceSystem;

    public override void Init(GameContext context)
    {
        resourceSystem = context.resourceSystem;
    }

    public void Store(int count)
    {
        resourceSystem.StoreResource(type, count);
    }

}
