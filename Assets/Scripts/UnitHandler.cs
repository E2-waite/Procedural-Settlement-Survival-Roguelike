using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UnitHandler : MonoSingleton<UnitHandler>
{
    public FollowerUnit selectedUnit = null;
    public FollowerUnit hoveringUnit = null;
    public GridTile hoveringTile = null;

    Pathfinding pathfinding;

    const int pathRange = 50;


    private void Start()
    {
        pathfinding = new Pathfinding();
    }

    public void Hover(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            return;
        }

        if (hit.collider.TryGetComponent<FollowerUnit>(out FollowerUnit unit))
        {
            hoveringUnit = unit;
            hoveringTile = null;
        }
        else
        {
            hoveringUnit = null;
            hoveringTile = Grid.Instance.getTile(hit.point);
        }
        
    }

    public void ClearHover()
    {
        hoveringUnit = null;
    }
    public void ClearSelection()
    {
        selectedUnit = null;
    }

    public void SelectUnit()
    {
        if (hoveringUnit == null)
            selectedUnit = null;
        else
            selectedUnit = hoveringUnit;
    }

    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        int size = pathRange * 2;

        bool[,] pathable = new bool[size, size];

        Vector2Int origin = new Vector2Int(
                            start.x - pathRange,
                            start.y - pathRange);

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2Int tilePos = new Vector2Int(origin.x + x, origin.y + y);

                GridTile tile = Grid.Instance.getTile(tilePos);

                if (tile != null && tile.Walkable())
                    pathable[x, y] = true;
                else
                    pathable[x, y] = false;
            }
        }

        Pathfinding.PathRequest request = new Pathfinding.PathRequest
        {
            size = size,
            start = start,
            target = target,
            origin = origin,
            pathable = pathable
        };

        return pathfinding.FindPath(request);
    }    

    public void MoveUnit()
    {
        if (!selectedUnit) return;

        if (hoveringTile != null)
        {
            Vector3 movePos = new Vector3(hoveringTile.position.x, 0.5f, hoveringTile.position.y);
            selectedUnit.MoveTo(movePos);
        }
    }
}
