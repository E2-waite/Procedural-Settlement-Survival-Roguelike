using UnityEngine;
using UnityEngine.UI;
using static CommandSystem;

public class ActionWidget : MonoBehaviour
{
    [SerializeField] Image commandMarker;
    [SerializeField] RectTransform rect;
    public RectTransform Rect => rect;

    public float markerSize = 10f, markerSelectSize = 20;
    public Image commandImage;

    public void Enable()
    {

    }

    public void Disable()
    {

    }

    public void Show(ActionTypeObject commandObj)
    {
        commandImage.sprite = commandObj.icon;
        commandImage.color = commandObj.color;
    }

    public void UpdateMarker(float diff, float max, bool selected)
    {
        float yPos = Mathf.Clamp(diff, -max, max);
        float scale = markerSize;

        if (selected)
        {
            scale = markerSelectSize;
        }

        commandMarker.rectTransform.anchoredPosition = new Vector2(0, yPos);
        commandMarker.rectTransform.sizeDelta = new Vector2(scale, scale);
    }

}
