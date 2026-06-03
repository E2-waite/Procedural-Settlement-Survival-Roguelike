using UnityEngine;
using UnityEngine.UI;
public class CommandPanel : MonoSingleton<CommandPanel>
{
    [SerializeField] private CommandWidget commandWidget;

    public void UpdateWidget(float diff)
    {
        commandWidget.UpdateSliders(diff);
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
    }
}
