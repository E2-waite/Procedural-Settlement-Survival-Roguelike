using Cinderwild.Pathfinding.Runtime;
using Cinderwild.World.Data;
using Cinderwild.World.Runtime;
using System.Collections.Generic;
using UnityEngine;

namespace Cinderwild.Gameplay.Agents
{
    // Handles agent pathfinding and movement
    public class AgentController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f, pathWeight = 2f, swarmWeight = 1f;
        [SerializeField] private float swarmRadius = 1f;
        private Vector3 targetPos = Vector3.zero;
        private Vector3 moveDir = Vector3.zero;
        [SerializeField] private List<Vector2Int> path = new List<Vector2Int>();
        private float reachedThresh = .01f;
        [SerializeField] private int pathIndex = 0;
        public bool ReachedTarget => pathIndex >= path.Count;
        public bool HasPath => !(path == null || path.Count == 0);
        private TileData targetTile;

        public TileData TargetTile => targetTile;
        private Agent agent;
        private bool pathRequested = false;
        private Vector3 facing = Vector3.zero;
        public Vector3 Facing => facing;
        private WorldSystem world;
        private PathfindingManager pathfinding;
        public void Init(Agent agent, WorldSystem world, PathfindingManager pathfinding)
        {
            this.world = world;
            this.agent = agent;
            this.pathfinding = pathfinding;

            TileData tile = world.GetTile(agent.Body.position);
            if (tile != null && tile.IsWalkable)
            {
                agent.Body.position += new Vector3(0, tile.WorldPosition.y, 0);
            }
        }

        public void MoveTo(TileData tile)
        {
            MoveTo(tile.WorldPosition);
        }

        public void MoveTo(Vector3 target)
        {
            if (!pathRequested)
            {
                // Validate target tile is walkable before requesting a path
                Vector2Int targetGrid = new Vector2Int(Mathf.FloorToInt(target.x), Mathf.FloorToInt(target.z));
                TileData tTile = world.GetTile(targetGrid);
                if (tTile == null || !tTile.IsWalkable)
                {
                    Debug.LogWarning($"MoveTo aborted: target tile not walkable or missing at {targetGrid}");
                    return;
                }

                pathfinding.RequestPath(agent.Body.position, target, SetPath);
                pathRequested = true;
                targetPos = target;
            }
        }

        private void SetPath(List<Vector2Int> path)
        {
            this.path = path;
            pathIndex = 0;
            pathRequested = false;
        }

        private void Update()
        {
            FollowPath();
            UpdateHeight();
        }

        private void FollowPath()
        {
            if (agent == null) return;

            float targetDist = Vector3.Distance(agent.Body.position, targetPos);
            if (path != null && path.Count > 0 && pathIndex < path.Count)
            {
                Vector2Int currentTarget = path[pathIndex];

                TileData tile = world.GetTile(currentTarget);

                Vector3 targetPos = new Vector3(currentTarget.x + .5f, 0, currentTarget.y + .5f);

                facing = (targetPos - agent.Body.position).normalized;

                //Debug.Log("Target tile: " + tile.Object.name + " pos: " + targetPos);
                Vector3 pathDir = (targetPos - agent.Body.position).normalized;
                //Vector3 swarmDir = SwarmDirection();
                Vector3 swarmDir = Vector3.zero;
                moveDir = (pathDir * pathWeight + swarmDir * swarmWeight).normalized;

                Vector3 movePos = agent.Body.position + (moveDir * moveSpeed * Time.deltaTime);
                movePos.y = tile.WorldPosition.y;
                agent.Body.position = movePos;

                // Remove y axis for threshold detection
                Vector3 flatPos = new Vector3(agent.Body.position.x, 0, agent.Body.position.z);
                if ((flatPos - targetPos).sqrMagnitude < reachedThresh)
                {
                    pathIndex++;
                }
            }
            //else if (targetDist < 5f && targetDist > 0.01f) // If we're close to the target position, move to the target
            //{
            //    moveDir = (targetPos - agent.Body.position).normalized;
            //    Vector3 movePos = agent.Body.position + (moveDir * moveSpeed * Time.deltaTime);
            //    movePos.y = 0;
            //    agent.Body.position = movePos;
            //}
            //else if (targetDist <= 0.01f)
            //{
            //    agent.Body.position = targetPos;
            //}
        }

        private void UpdateHeight()
        {
            // TODO: improve this.. feels inefficient 
            TileData tile = world.GetTile(agent.Body.position);
            agent.Body.position = new Vector3(agent.Body.position.x, tile.WorldPosition.y, agent.Body.position.z);
        }
    }
}
