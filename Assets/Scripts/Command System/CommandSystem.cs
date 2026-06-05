using System.Collections.Generic;
using UnityEngine;

public class CommandSystem
{
    public enum CommandType
    {
        Move,
        Build,
        Attack,
        Defend,
        Gather,
        Max
    }

    // Units in this list respond to player right-click commands.
    private List<FollowerUnit> commanding = new List<FollowerUnit>();
    public bool IsCommanding => commanding.Count > 0;
    private Player _player;

    public CommandSystem(Player player)
    {
        _player = player;
    }

    // Commands all following units to interact with the target
    public void Command(HoverTarget target)
    {
        if (target.IsTile)
        {
            foreach (FollowerUnit unit in commanding)
            {
                unit.Command(target.Tile);
            }
        }
        else if (target.IsEnemy)
        {
            foreach (FollowerUnit unit in commanding)
            {
                unit.Command(target.Enemy);
            }
        }
        else if (target.IsBuilding)
        {
            foreach (FollowerUnit unit in commanding)
            {
                unit.Command(target.Building);
            }
        }
    }
    
    // Commands all nearby follower units to start following the player
    public void StartCommanding(List<FollowerUnit> nearbyFollowers)
    {
        if (_player == null) return;

        // Nearby followers are claimed into the command group before receiving orders.
        foreach (FollowerUnit unit in nearbyFollowers)
        {
            if (unit == null) continue;

            if (!commanding.Contains(unit))
                commanding.Add(unit);
        }

        foreach (FollowerUnit unit in commanding)
        {
            unit.StartCommanding(_player);
        }
    }

    public void StartCommanding(FollowerUnit unit)
    {
        if (unit == null) return;

        if (!commanding.Contains(unit))
        {
            if (!commanding.Contains(unit))
                commanding.Add(unit);

            unit.StartCommanding(_player);
        }
    }

    public void StopCommanding(FollowerUnit unit)
    {
        if (commanding.Contains(unit))
        {
            commanding.Remove(unit);
        }
        unit.StopCommanding();
    }

    // Commands a unit to stop following the player
    public void StopCommanding()
    {
        foreach (FollowerUnit unit in commanding)
        {
            unit.StopCommanding();
        }
        commanding.Clear();
    }
    public void CommandFollow()
    {
        foreach (FollowerUnit unit in commanding)
        {
            unit.StartFollowing();
        }
    }
}
