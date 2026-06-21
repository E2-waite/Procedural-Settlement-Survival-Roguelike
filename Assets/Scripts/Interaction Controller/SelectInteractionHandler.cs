using UnityEngine;
using static InteractionController;
using static CommandSystem;
public class SelectInteractionHandler : IInteractionHandler
{
    private InteractionController controller;
    private CommandSystem commandSystem;
    private Player player;

    public SelectInteractionHandler(GameContext context)
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
        Vector3 lookDir = player.transform.position - target.Position;
        float angle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
        angle = Mathf.Round(angle / 45f) * 45f;

        Quaternion lookRot = Quaternion.Euler(0, angle, 0);
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
        if (time >= .25f && controller.Target.IsUnit)
        {
            commandSystem.StartCommanding(controller.Target.Unit);
        }
    }

    public void OnLeftUp(Vector2 diff, float time)
    {
        // Stops commanding units when LMB released and held for less than .25 seconds (tap) and not hovering over unit
        if (time < .25f)
        {
            if (!controller.Target.IsAgent)
                commandSystem.StopCommanding();
            else
            {
                if (controller.Target.IsUnit)
                {
                    Unit unit = controller.Target.Unit;

                    // Stop commanding current units if selecting different unit type
                    if (unit.Role == null && commandSystem.State != CommandState.Unit ||
                        unit.Role is WorkerRole && commandSystem.State != CommandState.Worker ||
                        unit.Role is FighterRole && commandSystem.State != CommandState.Fighter)
                    {
                        commandSystem.StopCommanding();
                    }

                    if (unit.Commanding)
                    {
                        commandSystem.StopCommanding(unit);
                    }
                    else
                    {
                        commandSystem.StartCommanding(unit);
                    }
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

    public void OnLookChanged(Quaternion rot)
    {
        commandSystem?.OnLookChanged(rot);
    }
}
