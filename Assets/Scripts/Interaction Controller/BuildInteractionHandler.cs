using UnityEngine;
using static InteractionController;

public class BuildInteractionHandler : IInteractionHandler
{
    private InteractionController controller;

    public BuildInteractionHandler(InteractionController controller)
    {
        this.controller = controller;
    }
    public void Enable()
    {
        BuildingSystem.Instance.SetEnabled(true);
    }

    public void Disable()
    {
        BuildingSystem.Instance.SetEnabled(false);
    }
    public void OnMouseMoved(Vector2 pos, Vector2 diff)
    {

    }

    public void OnLeftDown()
    {
        if (controller.Target.IsTile)
            BuildingSystem.Instance.TryPlace(controller.Target.Tile);
    }
    public void OnLeftHeld(Vector2 diff, float time)
    {

    }
    public void OnLeftUp(Vector2 diff, float time)
    {

    }
    public void OnRightDown()
    {
        controller.SetState(GameState.Control);
    }

    public void OnRightHeld(Vector2 diff, float time)
    {

    }
    public void OnRightUp(Vector2 diff, float time)
    {

    }
    public void OnEscape()
    {
        controller.SetState(GameState.Control);
    }
    public void OnFKey()
    {

    }
    public void OnMoveInput(Vector2 move)
    {

    }
}
