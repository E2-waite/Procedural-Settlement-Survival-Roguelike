using UnityEngine;
using static InteractionController;
using static CommandSystem;


public class CommandInteractionHandler : IInteractionHandler
{
    private InteractionController _controller;
    private CommandSystem _commandSystem;
    float commandDiff = 25f;
    HoverTarget clickTarget;
    bool commandSelected = false;

    public CommandInteractionHandler(InteractionController controller, CommandSystem commandSystem)
    {
        _controller = controller;
        _commandSystem = commandSystem;
    }

    public void Enable()
    {
        Cursor.visible = false;

        // Gets the target state when starting commanding
        clickTarget = new HoverTarget(_controller.Target);

        CommandType commandType = CommandType.Move;

        if (clickTarget.IsBuilding)
        {
            Building building = clickTarget.Building;
            if (!building.Built)
            {
                commandType = CommandType.Build;
            }
            else if (building is ResourceBuilding)
            {
                commandType = CommandType.Gather;
            }
        }
        else if (clickTarget.IsUnit)
        {
            commandType = CommandType.Attack;
        }

        CommandPanel.Instance.ShowWidget(commandType);
        CommandPanel.Instance.UpdateWidget(0, commandDiff, commandSelected);
        CommandPanel.Instance.SetWidgetPos(_controller.MousePos);
    }

    public void Disable()
    {
        CommandPanel.Instance.HideWidget();
    }

    public void OnHover(HoverTarget target)
    {

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
        commandSelected = diff.y > commandDiff || diff.y < -commandDiff;

        if (diff.y != 0 && time > 0.1f)
        {
            CommandPanel.Instance.UpdateWidget(diff.y, commandDiff, commandSelected);
        }
    }
    public void OnRightUp(Vector2 diff, float time)
    {
        if (diff.y > commandDiff)
        {
            // TODO: command to interact with object at mouse pos when the click started rather than the current position
            _commandSystem.Command(clickTarget);
        }
        else if (diff.y < -commandDiff)
        {
            _commandSystem.CommandFollow();
        }

        _controller.SetState(GameState.Control);
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
