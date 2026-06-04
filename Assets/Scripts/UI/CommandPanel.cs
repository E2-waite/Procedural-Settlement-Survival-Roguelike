using UnityEngine;
using UnityEngine.UI;
using static CommandSystem;

public class CommandPanel : MonoSingleton<CommandPanel>
{
    [SerializeField] private CommandWidget commandWidget;

    public void UpdateWidget(float diff, float max, bool selected)
    {
        commandWidget.UpdateMarker(diff, max, selected);
    }

    public void SetWidgetPos(Vector2 pos)
    {
        commandWidget.Rect.position = pos;
    }

    public void HideWidget()
    {
        commandWidget.gameObject.SetActive(false);

    }

    public void ShowWidget(CommandType commandType)
    {
        commandWidget.gameObject.SetActive(true);
        commandWidget.UpdateMarker(0, 0, false);
        commandWidget.Show(commandType);
    }
}
