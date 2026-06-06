using UnityEngine;
using UnityEngine.UIElements;

public class HoverTarget
{
    private GridTile hoveringTile = null;
    private Unit hoveringUnit = null;
    private Building hoveringBuilding = null;

    public bool IsTile => hoveringTile != null;
    public GridTile Tile => hoveringTile;

    public bool IsUnit => hoveringUnit != null;
    public bool IsFollower => hoveringUnit != null && hoveringUnit is FollowerUnit;
    public bool IsEnemy => hoveringUnit != null && hoveringUnit is EnemyUnit;
    public Unit Unit => hoveringUnit;
    public FollowerUnit Follower => (FollowerUnit)hoveringUnit;
    public EnemyUnit Enemy => (EnemyUnit)hoveringUnit;

    public bool IsBuilding => hoveringBuilding != null;
    public Building Building => hoveringBuilding;

    public HoverTarget()
    {
    }

    public HoverTarget(HoverTarget otherTarget)
    {
        if (otherTarget.IsTile)
        {
            Set(otherTarget.Tile);
        }
        else if (otherTarget.IsUnit)
        {
            Set(otherTarget.Unit);
        }
        else if (otherTarget.IsBuilding)
        {
            Set(otherTarget.Building);
        }
    }

    public void Update(RaycastHit hit, WorldGrid grid)
    {
        if (hit.transform.GetComponentInParent<Unit>() is Unit unit)
        {
            Set(unit);
        }
        else if (hit.transform.GetComponentInParent<Building>() is Building building)
        {
            Set(building);
            return;
        }
        else
        {
            GridTile tile = grid.GetTile(hit.point);
            if (tile != null)
            {
                Set(tile);
            }
        }
    }

    private void Set(GridTile tile)
    {
        if (tile != hoveringTile)
        {
            ClearOld();
            hoveringTile = tile;
            hoveringTile.SetHovering();
        }
    }

    private void Set(Unit unit)
    {
        if (unit != hoveringUnit)
        {
            ClearOld();
            hoveringUnit = unit;
            hoveringUnit.Highlight(true);
        }
    }

    private void Set(Building building)
    {
        if (building != hoveringBuilding)
        {
            ClearOld();
            hoveringBuilding = building;
            hoveringBuilding.SetHovering();
        }
    }

    private void ClearOld()
    {
        if (hoveringUnit != null)
        {
            hoveringUnit.Highlight(false);
            hoveringUnit = null;
        }
        else if (hoveringTile != null)
        {
            hoveringTile = null;
        }
        else if (hoveringBuilding != null)
        {
            hoveringBuilding = null;
        }
    }
}
