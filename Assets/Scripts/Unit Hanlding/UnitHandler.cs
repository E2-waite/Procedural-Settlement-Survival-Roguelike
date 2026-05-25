using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UnitHandler : MonoSingleton<UnitHandler>
{
    public GridTile hoveringTile = null;
    public List<FollowerUnit> followingUnits = new List<FollowerUnit>();

    public void SetFollowing(List<FollowerUnit> following)
    {
        followingUnits = new List<FollowerUnit>(following);
    }    

    public void Hover(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        hoveringTile = Grid.Instance.getTile(hit.point);
    }

    public void CommandUnits()
    {
        if (hoveringTile != null)
        {
            for (int i = 0; i < followingUnits.Count; i++)
            {
                followingUnits[i].Command(hoveringTile);
            }
        }
    }
}
