using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UnitMovement
{
    [SerializeField] public float moveSpeed = 10f, pathWeight = 2f, swarmWeight = 1f;
    public List<Vector2Int> path = new List<Vector2Int>();
    private float reachedThresh = .25f, swarmRadius = .5f;
    protected int pathIndex = 0;
    private Vector3 posOffset = new Vector3(0, 0.01f, 0);

    protected GridTile targetTile;

    public GridTile TargetTile => targetTile;
    public bool ReachedTarget => pathIndex >= path.Count;

    Unit unit;

    public void Init(Unit unit)
    {
        this.unit = unit;
    }

    public void SetTargetTile(GridTile tile)
    {
        targetTile = tile;
    }

    public void SetPath(List<Vector2Int> path)
    {
        this.path = path;
        pathIndex = 0;
    }

    public bool TargetReached => pathIndex >= path.Count;
    public bool HasPath => !(path == null || path.Count == 0);

    public void ClearPath()
    {
        if (path != null) path.Clear();
        pathIndex = 0;
    }

    Vector3 moveDir = Vector3.zero;

    public Vector3 MoveDir()
    {
        return moveDir;
    }

    public void FollowPath()
    {
        if (path != null && path.Count > 0 && pathIndex < path.Count)
        {
            Vector2Int currentTarget = path[pathIndex];

            Vector3 targetPos = new Vector3(currentTarget.x + .5f, 0, currentTarget.y + .5f);

            Vector3 pathDir = (targetPos - unit.transform.position).normalized;
            Vector3 swarmDir = SwarmDirection();

            moveDir = (pathDir * pathWeight + swarmDir * swarmWeight).normalized;

            Vector3 movePos = unit.transform.position + (moveDir * moveSpeed * Time.deltaTime);
            movePos.y = 0;
            unit.transform.position = movePos;

            if ((unit.transform.position - targetPos).sqrMagnitude < reachedThresh)
            {
                pathIndex++;
            }
        }
    }

    protected Vector3 SwarmDirection()
    {
        List<Unit> friendlyUnits = unit.GetNearbyFriendly();

        if (friendlyUnits == null) return Vector3.zero;

        Vector3 separation = Vector3.zero;
        foreach (Unit nearby in friendlyUnits)
        {
            if (nearby == null) continue;

            Vector3 diff = unit.transform.position - nearby.transform.position;
            float dist = diff.magnitude;

            if (dist < swarmRadius && dist > 0.0001f)
            {
                separation += diff.normalized / dist;
            }
        }

        return separation;
    }

}
