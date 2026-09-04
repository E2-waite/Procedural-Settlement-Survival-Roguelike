using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
public class CommandWidgetSlot : MonoBehaviour
{
    public Image Icon { get; private set; }
    public RectTransform Rect { get; private set; }

    public void Init()
    {
        if (transform.childCount == 0) return;
        Icon = transform.GetChild(0).GetComponent<Image>();
        Rect = GetComponent<RectTransform>();
    }
}
