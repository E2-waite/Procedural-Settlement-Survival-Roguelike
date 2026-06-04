using UnityEngine;
using static InteractionController;

public class ControlInteractionHandler : IInteractionHandler
{
    private InteractionController controller;

    public ControlInteractionHandler(InteractionController controller)
    {
        this.controller = controller;
    }

    public void Enable()
    {
        Cursor.visible = true;
    }

    public void Disable()
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
            CommandSystem.Instance.StartCommanding(controller.Target.Follower);
        }
    }
    public void OnLeftUp(Vector2 diff, float time)
    {
        // Stops commanding units when LMB released and held for less than .25 seconds (tap) and not hovering over unit
        if (time < .25f)
        {
            if (!controller.Target.IsUnit)
                CommandSystem.Instance.StopCommanding();
            else
            {
                if (controller.Target.IsFollower)
                {
                    FollowerUnit follower = controller.Target.Follower;

                    if (follower.Commanding)
                        CommandSystem.Instance.StopCommanding(follower);
                    else
                        CommandSystem.Instance.StartCommanding(follower);

                }
            }
        }
    }
    public void OnRightDown()
    {
        if (CommandSystem.Instance.IsCommanding)
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
        GameManager.Player.OnMove(move);
    }
}
