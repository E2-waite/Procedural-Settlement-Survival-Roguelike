using UnityEngine;
using UnityEngine.UI;
public class CommandPanel : MonoSingleton<CommandPanel>
{
    [SerializeField] private CommandWidget commandWidget;

    public void UpdateWidget(float diff, float max)
    {
        commandWidget.UpdateSliders(diff, max);
    }

    public void SetWidgetPos(Vector2 pos)
    {
        commandWidget.Rect.position = pos;
    }

    public void HideWidget()
    {
        commandWidget.gameObject.SetActive(false);

    }

    public void ShowWidget()
    {
        commandWidget.gameObject.SetActive(true);
        commandWidget.UpdateSliders(0, 0);
    }
}
