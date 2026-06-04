using System.Collections.Generic;
using UnityEngine;

public class CommandSystem : MonoSingleton<CommandSystem>
{
    // Units in this list respond to player right-click commands.
    private List<FollowerUnit> commanding = new List<FollowerUnit>();
     
    public void HandleHover(RaycastHit hit)
    {

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
    
    //// Commands all following units to interact with hovering unit
    //public void Command(Unit hoveringUnit)
    //{
    //    foreach (FollowerUnit unit in commanding)
    //    {
    //        unit.Command(hoveringUnit);
    //    }
    //}

    //// Commands all followers to interact with hovering building
    //public void Command(Building hoveringBuilding)
    //{

    //}

    // Commands all nearby follower units to start following the player
    public void StartCommanding(List<FollowerUnit> nearbyFollowers)
    {
        // Nearby followers are claimed into the command group before receiving orders.
        foreach (FollowerUnit unit in nearbyFollowers)
        {
            if (unit == null) continue;

            if (!commanding.Contains(unit))
                commanding.Add(unit);
        }

        foreach (FollowerUnit unit in commanding)
        {
            unit.StartCommanding(GameManager.Player);
        }
    }

    public void StartCommanding(FollowerUnit unit)
    {
        if (unit == null) return;

        if (!commanding.Contains(unit))
        {
            if (!commanding.Contains(unit))
                commanding.Add(unit);

            unit.StartCommanding(GameManager.Player);
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
