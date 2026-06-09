using UnityEngine;
using UnityEngine.UIElements;

public class HoverTarget
{
    private GridTile hoveringTile = null;
    private Agent hoveringAgent = null;
    private Building hoveringBuilding = null;

    public bool IsTile => hoveringTile != null;
    public GridTile Tile => hoveringTile;

    public bool IsAgent => hoveringAgent != null;
    public bool IsUnit => hoveringAgent != null && hoveringAgent is Unit;
    public bool IsEnemy => hoveringAgent != null && hoveringAgent is Enemy;
    public Agent Agent => hoveringAgent;
    public Unit Follower => (Unit)hoveringAgent;
    public Enemy Enemy => (Enemy)hoveringAgent;

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
        else if (otherTarget.IsAgent)
        {
            Set(otherTarget.Agent);
        }
        else if (otherTarget.IsBuilding)
        {
            Set(otherTarget.Building);
        }
    }

    public void Update(RaycastHit hit, WorldGrid grid)
    {
        if (hit.transform.GetComponentInParent<Agent>() is Agent agent)
        {
            Set(agent);
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

    private void Set(Agent agent)
    {
        if (agent != hoveringAgent)
        {
            ClearOld();
            hoveringAgent = agent;
            hoveringAgent.Highlight(true);
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
        if (hoveringAgent != null)
        {
            hoveringAgent.Highlight(false);
            hoveringAgent = null;
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
