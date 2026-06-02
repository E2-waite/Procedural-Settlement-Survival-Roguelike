using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building")]
public class BuildingObject : ScriptableObject
{
    public GameObject prefab;
    public Vector2Int size;
    public Sprite icon;

    [SerializeField] ResourceStorage cost = new ResourceStorage();

    public bool CanAfford()
    {
        for (ResourceNode.Type i = ResourceNode.Type.Wood; i < ResourceNode.Type.Max; i++)
        {
            if (ResourceSystem.Instance.GetResourceCount(i) < cost.Get(i))
                return false;
        }

        return true;
    }

    public void ConsumeResources()
    {
        for (ResourceNode.Type i = ResourceNode.Type.Wood; i < ResourceNode.Type.Max; i++)
        {
            ResourceSystem.Instance.ConsumeResource(i, cost.Get(i));
        }
    }
}
