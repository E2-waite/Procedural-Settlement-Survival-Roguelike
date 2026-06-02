using System.Collections.Generic;
using UnityEngine;

public class UnitSystem : MonoSingleton<UnitSystem>
{
    private UnitStorage storage = new UnitStorage();

    public UnitStorage Storage => storage;

    public void HandleHover(RaycastHit hit)
    {

    }

    public void Command(GridTile hoveringTile)
    {
        foreach (FollowerUnit unit in storage.Followers)
        {
            unit.Command(hoveringTile);
        }
    }
    
    public void Command(Unit hoveringUnit)
    {
        foreach (FollowerUnit unit in storage.Followers)
        {
            unit.Command(hoveringUnit);
        }
    }

    // Command following units to interact with the passed building
    public void Command(Building hoveringBuilding)
    {

    }

    // Tells all nearby followers to start following the player
    public void StartFollowing()
    {
        // Tells nearby units to start following
        foreach (FollowerUnit nearby in storage.Nearby)
        {
            storage.AddFollowing(nearby);
            nearby.StartFollowing(GameManager.Player);
        }

        storage.ClearNearby();
    }

    public void StopFollowing(FollowerUnit unit)
    {
        storage.RemoveFollowing(unit);
        unit.StopFollowing();
    }
}
