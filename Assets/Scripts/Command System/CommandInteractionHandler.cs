using UnityEngine;
using static InteractionController;
using static CommandSystem;
using static GlobalDefs;
using System.Data;

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
        Cursor.visible = false;

        // Gets the target state when starting commanding
        clickTarget = new HoverTarget(controller.Target);

        InteractType commandType = InteractType.Move;

        switch (commandSystem.State)
        {
            case CommandState.Unit:
                commandType = UnitInteractType();
                break;
            case CommandState.Worker:
                commandType = WorkerInteractType();
                break;
            case CommandState.Fighter:
                commandType = FighterInteractType();
                break;
        }

        commandPanel.ShowWidget(commandType);
        commandPanel.UpdateWidget(0, commandDiff, commandSelected);
        commandPanel.SetWidgetPos(controller.MousePos);
    }

    InteractType UnitInteractType()
    {
        if (clickTarget.IsBuilding && clickTarget.Building is ConvertBuilding)
        {
            return InteractType.Convert;
        }
        return InteractType.Move;
    }

    InteractType WorkerInteractType()
    {
        if (clickTarget.IsBuilding && clickTarget.Building.Owner == Faction.Unit)
        {
            if (!clickTarget.Building.Built)
            {
                return InteractType.Build;
            }
            else if (clickTarget.Building is ResourceBuilding)
            {
                return InteractType.Gather;
            }
            else if (clickTarget.Building is ConvertBuilding)
            {
                return InteractType.Convert;
            }
        }
        return InteractType.Move;
    }

    InteractType FighterInteractType()
    {
        if (clickTarget.IsEnemy)
        {
            return InteractType.Attack;
        }
        else if (clickTarget.IsBuilding)
        {
            if (clickTarget.Building.Owner == Faction.Enemy)
            {
                return InteractType.Attack;
            }
            else if (clickTarget.Building is ConvertBuilding)
            {
                return InteractType.Convert;
            }
        }
        return InteractType.Defend;
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

        controller.SetState(GameState.Select);
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
    public void OnLookChanged(Quaternion rot)
    {

    }
}
