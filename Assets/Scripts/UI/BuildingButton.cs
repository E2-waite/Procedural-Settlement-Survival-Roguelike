using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public Button button;

    public void Setup(BuildingObject building, int index)
    {
        // Setup icons, etc.

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            BuildingHandler.Instance.SelectBuilding(index);
        });
    }
}
