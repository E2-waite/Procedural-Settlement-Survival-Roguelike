using UnityEngine;
using static InteractionController;

public class CommandInteractionHandler : IInteractionHandler
{
    private InteractionController controller;
    float commandDiff = 25f;
    HoverTarget clickTarget;

    public CommandInteractionHandler(InteractionController controller)
    {
        this.controller = controller;
    }
    public void Enable()
    {
        // Gets the target state when starting commanding
        clickTarget = new HoverTarget(controller.Target);

        CommandPanel.Instance.ShowWidget();
        CommandPanel.Instance.UpdateWidget(0, commandDiff);
        CommandPanel.Instance.SetWidgetPos(controller.MousePos);
    }

    public void Disable()
    {
        CommandPanel.Instance.HideWidget();
    }
    public void OnMouseMoved(Vector2 pos, Vector2 diff)
    {

    }

    public void OnLeftDown()
    {

    }
    public void OnLeftHeld(Vector2 diff, float time)
    {
        
    }

    public void OnLeftUp(Vector2 diff, float time)
    {
        
    }
    public void OnRightDown()
    {

    }
    public void OnRightHeld(Vector2 diff, float time)
    {
        if (diff.y != 0 && time > 0.1f)
        {
            CommandPanel.Instance.UpdateWidget(diff.y, commandDiff);
        }
    }
    public void OnRightUp(Vector2 diff, float time)
    {
        if (diff.y > commandDiff)
        {
            // TODO: command to interact with object at mouse pos when the click started rather than the current position
            CommandSystem.Instance.Command(clickTarget);
        }
        else if (diff.y < -commandDiff)
        {
            CommandSystem.Instance.CommandFollow();
        }

        controller.SetState(GameState.Control);
    }
    public void OnEscape()
    {

    }
    public void OnFKey()
    {

    }
    public void OnMoveInput(Vector2 move)
    {

    }
}
