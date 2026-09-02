using UnityEngine;

namespace Cinderwild.Command.UI
{
    public class CommandPanel : MonoBehaviour
    {
        [SerializeField] private CommandWidget commandWidget;
        //public CommandTypeCatalog actionCatalog;

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

        //public void ShowWidget(CommandType type)
        //{
        //    commandWidget.gameObject.SetActive(true);
        //    commandWidget.UpdateMarker(0, 0, false);
        //    commandWidget.Show(actionCatalog.GetObject(type));
        //}
    }
}
