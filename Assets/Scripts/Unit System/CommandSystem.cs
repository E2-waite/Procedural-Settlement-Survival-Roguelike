using System.Collections.Generic;
using UnityEngine;

public class CommandSystem : MonoSingleton<CommandSystem>
{
    // Units in this list respond to player right-click commands.
    private List<FollowerUnit> following = new List<FollowerUnit>();
     
    public void HandleHover(RaycastHit hit)
    {

    }

    // Commands all following units to interact with hovering tile
    public void Command(GridTile hoveringTile)
    {
        foreach (FollowerUnit unit in following)
        {
            unit.Command(hoveringTile);
        }
    }
    
    // Commands all following units to interact with hovering unit
    public void Command(Unit hoveringUnit)
    {
        foreach (FollowerUnit unit in following)
        {
            unit.Command(hoveringUnit);
        }
    }

    // Commands all followers to interact with hovering building
    public void Command(Building hoveringBuilding)
    {

    }

    // Commands all nearby follower units to start following the player
    public void CommandStartFollowing(List<FollowerUnit> nearbyFollowers)
    {
        // Nearby followers are claimed into the command group before receiving orders.
        foreach (FollowerUnit nearby in nearbyFollowers)
        {
            if (nearby == null) continue;

            if (!following.Contains(nearby))
                following.Add(nearby);

            nearby.StartFollowing(GameManager.Player);
        }
    }

    // Commands a unit to stop following the player
    public void CommandStopFollowing(FollowerUnit unit)
    {
        following.Remove(unit);
        unit.StopFollowing();
    }
}
