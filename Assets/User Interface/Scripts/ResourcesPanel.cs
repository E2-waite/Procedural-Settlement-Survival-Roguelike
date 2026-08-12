using TMPro;
using UnityEngine;

public class ResourcesPanel : MonoBehaviour
{
    public TMP_Text[] resourceCounts = new TextMeshPro[(int)ResourceNode.Type.Max];

    public void UpdateCount(ResourceNode.Type type, int count)
    {
        TMP_Text resourceText = resourceCounts[(int)type];
        if (resourceText != null)
            resourceText.text = count.ToString();
    }
}
