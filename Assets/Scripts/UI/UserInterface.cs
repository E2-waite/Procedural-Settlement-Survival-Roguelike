using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public BuildPanel buildPanel;
    public CommandPanel commandPanel;
    public ResourcesPanel resourcesPanel;
    public void Init(InteractionController interaction, BuildingCatalog buildingCatalog)
    {
        buildPanel.Init(interaction, buildingCatalog);
    }

}
