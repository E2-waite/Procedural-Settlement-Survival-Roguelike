using UnityEngine;
using UnityEngine.UI;
using static CommandSystem;

public class CommandPanel : MonoBehaviour
{
    [SerializeField] private CommandWidget commandWidget;
    public CommandTypeCatalog commandCatalog;
    public void Init(GameContext context)
    {
        
    }

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

    public void ShowWidget(InteractType type)
    {
        commandWidget.gameObject.SetActive(true);
        commandWidget.UpdateMarker(0, 0, false);
        commandWidget.Show(commandCatalog.GetObject(type));
    }
}
