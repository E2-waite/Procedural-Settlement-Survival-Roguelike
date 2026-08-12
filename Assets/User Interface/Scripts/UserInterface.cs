using UnityEngine;

public class UserInterface : MonoBehaviour
{
    public BuildPanel buildPanel;
    public ActionPanel commandPanel;
    public ResourcesPanel resourcesPanel;
    public void Init(GameContext context)
    {
        buildPanel.Init(context);
    }

}
