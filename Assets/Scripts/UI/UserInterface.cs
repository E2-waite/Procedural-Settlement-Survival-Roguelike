using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public BuildPanel buildPanel;
    public CommandPanel commandPanel;
    public ResourcesPanel resourcesPanel;
    public void Init(GameContext context)
    {
        buildPanel.Init(context);
    }

}
