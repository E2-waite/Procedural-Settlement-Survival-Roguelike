using UnityEngine;
using UnityEngine.UI;

public class CommandWidget : MonoBehaviour
{
    //[SerializeField] Image commandSlider;
    //[SerializeField] Image followSlider;
    [SerializeField] Image commandMarker;
    [SerializeField] RectTransform rect;
    public RectTransform Rect => rect;

    public void UpdateSliders(float diff, float max)
    {
        float yPos = Mathf.Clamp(diff, -max, max);
        float scale = 75;

        if (yPos >= max || yPos <= -max)
        {
            scale = 100;
        }

        commandMarker.rectTransform.anchoredPosition = new Vector2(0, yPos);
        commandMarker.rectTransform.sizeDelta = new Vector2(scale, scale);
    }
}
