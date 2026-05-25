using System.Collections.Generic;
using UnityEngine;
using static Pathfinding;

public class PathAgent : MonoBehaviour
{
    const int pathRange = 50;

    public float moveSpeed = 10f;
    public float reachedThresh = .05f;
    public List<Vector2Int> currentPath = new List<Vector2Int>();
    private float pathInterval = 0.5f, repathTimer = 0;

    protected int pathIndex = 0;
    protected bool pathRequested = false;
    protected Vector3 targetPos;

    protected void RequestPath(Vector2Int start, Vector2Int target, Vector3 worldPos)
    {
        targetPos = worldPos;

        pathRequested = true;
        int size = pathRange * 2;

        bool[,] pathable = new bool[size, size];

        Vector2Int origin = new Vector2Int(
                            start.x - pathRange,
                            start.y - pathRange);

        // Define pathing area
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

        // Construct the pathing request
        Pathfinding.PathRequest request = new Pathfinding.PathRequest
        {
            size = size,
            start = start,
            target = target,
            origin = origin,
            pathable = pathable,
            callback = (path) =>
            {
                // Updates the path on callback
                currentPath = path;
                pathIndex = 0;
                pathRequested = false;
            }
        };

        // Send pathfinding request
        PathfindingHandler.Instance.RequestPath(request);
    }

    protected void FollowPath()
    {
        if (currentPath != null && currentPath.Count > 0)
        {
            Vector2Int currentTarget = currentPath[pathIndex];

            Vector3 currentTargetPos = new Vector3(currentTarget.x + .5f, .5f, currentTarget.y + .5f);
            transform.position = Vector3.MoveTowards(transform.position, currentTargetPos, moveSpeed * Time.deltaTime);

            if ((transform.position - currentTargetPos).sqrMagnitude < reachedThresh)
            {
                pathIndex++;

                if (pathIndex >= currentPath.Count)
                {
                    currentPath.Clear();
                    pathIndex = 0;
                }
            }
        }
    }

    protected bool HasPath()
    {
        return !(currentPath.Count == 0 || currentPath == null);
    }
}
