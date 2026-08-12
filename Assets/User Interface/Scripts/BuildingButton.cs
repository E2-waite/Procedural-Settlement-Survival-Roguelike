using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public Image image;
    public Button button;
    public TMP_Text text;
    public void Setup(InteractionController interaction, BuildingObject building)
    {
        // Setup icons, etc.
        text.text = building.name;
        if (building.icon != null)
            image.sprite = building.icon;
        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            interaction.EnterBuildState(building);
        });
    }
}
