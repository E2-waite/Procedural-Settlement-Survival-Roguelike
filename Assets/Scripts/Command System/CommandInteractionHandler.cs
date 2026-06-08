using UnityEngine;
using static InteractionController;
using static CommandSystem;


public class CommandInteractionHandler : IInteractionHandler
{
    private InteractionController controller;
    private CommandSystem commandSystem;
    private CommandPanel commandPanel;
    float commandDiff = 60f;
    HoverTarget clickTarget;
    bool commandSelected = false;

    public CommandInteractionHandler(GameContext context)
    {
        controller = context.interactionController;
        commandSystem = context.commandSystem;
        commandPanel = context.commandPanel;
    }

    public void Enable()
    {
        if (commandSystem.State == CommandState.None) return;

        Cursor.visible = false;

        // Gets the target state when starting commanding
        clickTarget = new HoverTarget(controller.Target);

        CommandType commandType = CommandType.Move;
        if (commandSystem.State == CommandState.Fighter) commandType = CommandType.Defend;

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

        commandPanel.ShowWidget(commandType);
        commandPanel.UpdateWidget(0, commandDiff, commandSelected);
        commandPanel.SetWidgetPos(controller.MousePos);
    }

    public void Disable()
    {
        commandPanel.HideWidget();
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
            commandPanel.UpdateWidget(diff.y, commandDiff, commandSelected);
        }
    }
    public void OnRightUp(Vector2 diff, float time)
    {
        if (diff.y > commandDiff)
        {
            // TODO: command to interact with object at mouse pos when the click started rather than the current position
            commandSystem.Command(clickTarget);
        }
        else if (diff.y < -commandDiff)
        {
            commandSystem.CommandFollow();
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
