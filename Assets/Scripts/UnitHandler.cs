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

    public void MoveUnit()
    {
        if (hoveringTile != null)
        {
            Vector3 movePos = new Vector3(hoveringTile.position.x, 0.5f, hoveringTile.position.y);

            for (int i = 0; i < followingUnits.Count; i++)
            {
                followingUnits[i].MoveTo(movePos);
            }
        }
    }
}
