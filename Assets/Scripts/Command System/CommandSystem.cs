using System.Collections.Generic;
using UnityEngine;

public class CommandSystem
{
    public enum CommandState
    {
        None,
        Worker,
        Fighter
    }

    // Currently just for the command widget 
    public enum CommandType
    {
        Move,
        Build,
        Attack,
        Defend,
        Gather,
        Max
    }

    private CommandState commandState = CommandState.None;
    public CommandState State => commandState;
    // Units in this list respond to player right-click commands.
    private List<Unit> commanding = new List<Unit>();
    public bool IsCommanding => commanding.Count > 0;
    private Player player;

    public CommandSystem(GameContext context)
    {
        player = context.player;

    }

    private void SetState(CommandState state)
    {
        if (state != commandState)
        {
            commandState = state;
        }
    }

    // Commands all following units to interact with the target
    public void Command(HoverTarget target)
    {
        if (target.IsTile)
        {
            foreach (Unit unit in commanding)
            {
                unit.Command(target.Tile);
            }
        }
        else if (target.IsEnemy)
        {
            foreach (Unit unit in commanding)
            {
                unit.Command(target.Enemy);
            }
        }
        else if (target.IsBuilding)
        {
            foreach (Unit unit in commanding)
            {
                unit.Command(target.Building);
            }
        }
    }
    
    // Commands all nearby follower units to start following the player
    public void StartCommanding(List<Unit> nearbyFollowers)
    {
        if (player == null) return;

        // Nearby followers are claimed into the command group before receiving orders.
        foreach (Unit unit in nearbyFollowers)
        {
            if (unit == null) continue;

            if (!commanding.Contains(unit))
                commanding.Add(unit);
        }

        foreach (Unit unit in commanding)
        {
            unit.StartCommanding(player);
        }
    }

    public void StartCommanding(Unit unit)
    {
        if (unit == null) return;

        if (unit.Role == null) SetState(CommandState.None);
        else if (unit.Role is WorkerRole) SetState(CommandState.Worker);
        else if (unit is FighterUnit) SetState(CommandState.Fighter);

        if (!commanding.Contains(unit))
        {
            if (!commanding.Contains(unit))
                commanding.Add(unit);

            unit.StartCommanding(player);
        }
    }

    // Stops commanding a specific unit
    public void StopCommanding(Unit unit)
    {
        if (commanding.Contains(unit))
        {
            commanding.Remove(unit);
        }
        unit.StopCommanding();

        if (commanding.Count == 0) SetState(CommandState.None);
    }

    // Stops commanding all units
    public void StopCommanding()
    {
        foreach (Unit unit in commanding)
        {
            unit.StopCommanding();
        }
        commanding.Clear();
        SetState(CommandState.None);
    }
    public void CommandFollow()
    {
        foreach (Unit unit in commanding)
        {
            unit.StartFollowing();
        }
    }
}
