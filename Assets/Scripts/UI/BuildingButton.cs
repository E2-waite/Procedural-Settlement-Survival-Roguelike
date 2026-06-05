using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour
{
    public Button button;
    public TMP_Text text;
    public void Setup(InteractionController interaction, BuildingObject building)
    {
        // Setup icons, etc.
        text.text = building.name;

        button.onClick.RemoveAllListeners();

        button.onClick.AddListener(() =>
        {
            interaction.EnterBuildState(building);
        });
    }
}
