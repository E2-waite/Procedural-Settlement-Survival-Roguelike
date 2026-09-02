using Cinderwild.Gameplay.Agents;
using Cinderwild.Pathfinding.Runtime;
using Cinderwild.World.Data;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


// Handles agent pathfinding and movement
public class AgentController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f, pathWeight = 2f, swarmWeight = 1f;
    [SerializeField] private float swarmRadius = 1f;
    private Vector3 targetPos = Vector3.zero;
    private Vector3 moveDir = Vector3.zero;
    private List<Vector2Int> path = new List<Vector2Int>();
    private float reachedThresh = .01f;
    private int pathIndex = 0;
    public bool ReachedTarget => pathIndex >= path.Count;
    public bool HasPath => !(path == null || path.Count == 0);
    private TileData targetTile;

    public TileData TargetTile => targetTile;
    private Agent agent;
    private bool pathRequested = false;

    public void Init(Agent agent)
    {
        this.agent = agent;
    }

    public void MoveTo(TileData tile)
    {
    }

    public void MoveTo(Vector3 target)
    {
        if (!pathRequested)
        {
            PathfindingSystem.RequestPath(transform.position, target, SetPath);
            pathRequested = true;
        }
    }

    private void SetPath(List<Vector2Int> path)
    {
        this.path = path;
        pathIndex = 0;
    }

    private void Update()
    {
        //FollowPath();
    }

    private void FollowPath()
    {
        float targetDist = Vector3.Distance(agent.Body.position, targetPos);
        if (path != null && path.Count > 0 && pathIndex < path.Count)
        {
            Vector2Int currentTarget = path[pathIndex];

            Vector3 targetPos = new Vector3(currentTarget.x + .5f, 0, currentTarget.y + .5f);

            Vector3 pathDir = (targetPos - agent.Body.position).normalized;
            //Vector3 swarmDir = SwarmDirection();
            Vector3 swarmDir = Vector3.zero;
            moveDir = (pathDir * pathWeight + swarmDir * swarmWeight).normalized;

            Vector3 movePos = agent.Body.position + (moveDir * moveSpeed * Time.deltaTime);
            movePos.y = 0;
            agent.Body.position = movePos;

            if ((agent.Body.position - targetPos).sqrMagnitude < reachedThresh)
            {
                pathIndex++;
            }
        }
        else if (targetDist < 5f && targetDist > 0.01f) // If we're close to the target position, move to the target
        {
            moveDir = (targetPos - agent.Body.position).normalized;
            Vector3 movePos = agent.Body.position + (moveDir * moveSpeed * Time.deltaTime);
            movePos.y = 0;
            agent.Body.position = movePos;
        }
        else if (targetDist <= 0.01f)
        {
            agent.Body.position = targetPos;
        }
    }
}
