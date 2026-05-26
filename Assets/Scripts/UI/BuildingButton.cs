using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public Button button;
    public TMP_Text text;
    public void Setup(BuildingObject building, int index)
    {
        // Setup icons, etc.
        text.text = building.name;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            BuildingHandler.Instance.SelectBuilding(index);
        });
    }
}
