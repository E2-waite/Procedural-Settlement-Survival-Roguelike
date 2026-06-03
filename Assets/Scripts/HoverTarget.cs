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
    public bool IsEnemy => hoveringUnit != null && hoveringUnit is FollowerUnit;
    public Unit Unit => hoveringUnit;
    public FollowerUnit Follower => (FollowerUnit)hoveringUnit;
    public EnemyUnit Enemy => (EnemyUnit)hoveringUnit;

    public bool IsBuilding => hoveringBuilding != null;
    public Building Building => hoveringBuilding;

    public void Update(RaycastHit hit)
    {
        switch (hit.transform.tag)
        {
            case "Unit":
                Set(hit.transform.GetComponent<Unit>());
                break;
            case "Tile":
                Set(WorldManager.grid.GetTile(hit.point));
                break;
            case "Building":
                Set(hit.transform.GetComponent<Building>());
                break;
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
            hoveringUnit.SetHovering();
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
            hoveringUnit.ClearHovering();
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
