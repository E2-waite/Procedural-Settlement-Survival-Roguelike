using UnityEngine;
using UnityEngine.UI;

public class CommandWidget : MonoBehaviour
{
    [SerializeField] Image commandSlider;
    [SerializeField] Image followSlider;
    [SerializeField] RectTransform rect;
    public RectTransform Rect => rect;

    public void UpdateSliders(float diff)
    {
        float barFill = 0;

        if (diff > 0 || diff < 0)
        {
            barFill = Mathf.Clamp01(Mathf.Abs(diff / 10));
        }

        if (diff > 0)
        {
            commandSlider.fillAmount = barFill;
            followSlider.fillAmount = 0;
        }
        else if (diff < 0)
        {
            followSlider.fillAmount = barFill;
            commandSlider.fillAmount = 0;
        }
    }
}
