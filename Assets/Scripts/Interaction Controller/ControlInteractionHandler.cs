using UnityEngine;
using static InteractionController;
using static CommandSystem;
public class ControlInteractionHandler : IInteractionHandler
{
    private InteractionController controller;
    private CommandSystem commandSystem;
    private Player player;

    public ControlInteractionHandler(GameContext context)
    {
        controller = context.interactionController;
        commandSystem = context.commandSystem;
        player = context.player;
    }

    public void Enable()
    {
        Cursor.visible = true;
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
        // Starts commanding hoving unit when LMB held for over .25 seconds
        if (time >= .25f && controller.Target.IsFollower)
        {
            commandSystem.StartCommanding(controller.Target.Follower);
        }
    }
    public void OnLeftUp(Vector2 diff, float time)
    {
        // Stops commanding units when LMB released and held for less than .25 seconds (tap) and not hovering over unit
        if (time < .25f)
        {
            if (!controller.Target.IsUnit)
                commandSystem.StopCommanding();
            else
            {
                if (controller.Target.IsFollower)
                {
                    FollowerUnit follower = controller.Target.Follower;

                    // Stop commanding current units if selecting different unit type
                    if (follower is FollowerUnit && commandSystem.State == CommandState.Fighter ||
                        follower is FighterUnit && commandSystem.State == CommandState.Worker)
                    {
                        commandSystem.StopCommanding();
                    }

                    if (follower.Commanding)
                        commandSystem.StopCommanding(follower);
                    else
                        commandSystem.StartCommanding(follower);

                }
            }
        }
    }
    public void OnRightDown()
    {
        if (commandSystem.IsCommanding)
            controller.SetState(GameState.Command);
    }
    public void OnRightHeld(Vector2 diff, float time)
    {
    }
    public void OnRightUp(Vector2 diff, float time)
    {
    }
    public void OnEscape()
    {
    }
    public void OnFKey()
    {
    }
    public void OnMoveInput(Vector2 move)
    {
        player.OnMove(move);
    }
}
