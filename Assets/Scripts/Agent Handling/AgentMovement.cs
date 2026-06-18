using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentMovement
{
    [SerializeField] public float moveSpeed = 5f, pathWeight = 2f, swarmWeight = 1f;
    public List<Vector2Int> path = new List<Vector2Int>();
    private float reachedThresh = .01f;
    [SerializeField] private float swarmRadius = 1f;
    protected int pathIndex = 0;
    private Vector3 posOffset = new Vector3(0, 0.01f, 0);
    private Vector3 targetPosition = Vector3.zero;

    protected GridTile targetTile;

    public GridTile TargetTile => targetTile;
    public bool ReachedTarget => pathIndex >= path.Count;

    Agent agent;

    public void Init(Agent agent)
    {
        this.agent = agent;
    }

    public void SetTargetTile(GridTile tile)
    {
        targetTile = tile;
    }

    public void SetTargetPos(Vector3 pos)
    {
        targetPosition = pos;
    }

    public void SetPath(List<Vector2Int> path)
    {
        this.path = path;
        pathIndex = 0;
    }

    public bool TargetReached => agent.transform.position == targetPosition;
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
        float targetDist = Vector3.Distance(agent.transform.position, targetPosition);
        if (path != null && path.Count > 0 && pathIndex < path.Count)
        {
            Vector2Int currentTarget = path[pathIndex];

            Vector3 targetPos = new Vector3(currentTarget.x + .5f, 0, currentTarget.y + .5f);

            Vector3 pathDir = (targetPos - agent.transform.position).normalized;
            //Vector3 swarmDir = SwarmDirection();
            Vector3 swarmDir = Vector3.zero;
            moveDir = (pathDir * pathWeight + swarmDir * swarmWeight).normalized;

            Vector3 movePos = agent.transform.position + (moveDir * moveSpeed * Time.deltaTime);
            movePos.y = 0;
            agent.transform.position = movePos;

            if ((agent.transform.position - targetPos).sqrMagnitude < reachedThresh)
            {
                pathIndex++;
            }
        }
        else if (targetDist < 5f && targetDist > 0.01f) // If we're close to the target position, move to the target
        {
            moveDir = (targetPosition - agent.transform.position).normalized;
            Vector3 movePos = agent.transform.position + (moveDir * moveSpeed * Time.deltaTime);
            movePos.y = 0;
            agent.transform.position = movePos;
        }
        else if (targetDist <= 0.01f)
        {
            agent.transform.position = targetPosition;
        }
    }

    protected Vector3 SwarmDirection()
    {
        if (!agent.HasSquad) return Vector3.zero;

        Vector3 separation = Vector3.zero;
        foreach (Agent nearby in agent.Squad.Agents)
        {
            if (nearby == null || nearby == agent) continue;

            Vector3 diff = agent.transform.position - nearby.transform.position;
            float dist = diff.magnitude;

            if (dist < swarmRadius && dist > 0.0001f)
            {
                separation += diff.normalized / dist;
            }
        }

        return separation;
    }

}
