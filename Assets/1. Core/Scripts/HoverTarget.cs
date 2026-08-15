using UnityEngine;
using Cinderwild.WorldBuilder.Data;
using Cinderwild.WorldBuilder.Data;

public class HoverTarget
{
    private TileData hoveringTile = null;
    private Agent hoveringAgent = null;
    private Building hoveringBuilding = null;
    private Vector3 hoveringPos = Vector3.zero;
    public Vector3 Position => hoveringPos;
    public bool IsTile => hoveringTile != null;
    public TileData Tile => hoveringTile;

    public bool IsAgent => hoveringAgent != null;
    public bool IsUnit => hoveringAgent != null && hoveringAgent is Unit;
    public bool IsEnemy => hoveringAgent != null && hoveringAgent is Enemy;
    public Agent Agent => hoveringAgent;
    public Unit Unit => (Unit)hoveringAgent;
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
        hoveringPos = otherTarget.Position;
    }

    public bool Update(RaycastHit hit, World world)
    {
        hoveringPos = hit.point;

        if (hit.transform.GetComponentInParent<Agent>() is Agent agent)
        {
            return Set(agent);
        }
        else if (hit.transform.GetComponentInParent<Building>() is Building building)
        {
            return Set(building);
        }
        else
        {
            TileData tile = grid.GetTile(hit.point);
            if (tile != null)
            {
                return Set(tile);
            }
        }

        return false;
    }

    private bool Set(TileData tile)
    {
        if (tile != hoveringTile)
        {
            ClearOld();
            hoveringTile = tile;
            return true;
        }
        return false;
    }

    private bool Set(Agent agent)
    {
        if (agent != hoveringAgent)
        {
            ClearOld();
            hoveringAgent = agent;
            return true;
        }
        return false;
    }

    private bool Set(Building building)
    {
        if (building != hoveringBuilding)
        {
            ClearOld();
            hoveringBuilding = building;
            return true;
        }
        return false;
    }

    private void ClearOld()
    {
        if (hoveringAgent != null)
        {
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
