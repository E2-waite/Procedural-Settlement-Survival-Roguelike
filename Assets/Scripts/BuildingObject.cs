using UnityEngine;

[CreateAssetMenu(fileName = "Building", menuName = "Scriptable Objects/Building")]
public class BuildingObject : ScriptableObject
{
    public GameObject prefab;
    public Vector2Int size;
    public Sprite icon;

    public int[] resourceCost = new int[(int)ResourceNode.Type.Max];

    public bool CanAfford()
    {
        for (int i = 0; i < (int)ResourceNode.Type.Max; i++)
        {
            if (ResourceHandler.Instance.GetResourceCount(i) < resourceCost[i])
                return false;
        }

        return true;
    }

    public void ConsumeResources()
    {
        for (int i = 0; i < (int)ResourceNode.Type.Max; i++)
        {
            ResourceHandler.Instance.ConsumeResource(i, resourceCost[i]);
        }
    }
}
