using UnityEngine;
using static InteractionController;
using static CommandSystem;
using static GlobalDefs;
using System.Data;

public class CommandInteractionHandler : IInteractionHandler
{
    private InteractionController controller;
    private CommandSystem commandSystem;
    //private ActionPanel commandPanel;


    public CommandInteractionHandler(GameContext context)
    {
        controller = context.interactionController;
        commandSystem = context.commandSystem;
    }

    public void Enable()
    {

    }

    

    public void Disable()
    {
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
        commandSystem?.AimFormation(true);
    }
    public void OnRightUp(Vector2 diff, float time)
    {
        commandSystem?.AimFormation(false);
        commandSystem?.CommandMove(controller.Target);
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
        commandSystem?.UpdateLookRot(rot);
    }
}
