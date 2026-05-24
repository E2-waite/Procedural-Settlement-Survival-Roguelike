using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static Pathfinding;
using static UnityEngine.GraphicsBuffer;

public class FollowerUnit : MonoBehaviour
{
    enum UnitState
    {
        Idle,
        Moving,
        Following
    }

    public float moveSpeed = 10f;
    UnitState state;
    PlayerController player;

    private Camera cam;

    public List<Vector2Int> currentPath = new List<Vector2Int>();
    private Vector2Int lastTarget = new Vector2Int();
    private Vector2Int currentTarget = new Vector2Int();

    private float pathInterval = 0.5f, repathTimer = 0;

    private int pathIndex = 0;
    private bool pathRequested = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = UnitState.Idle;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        // Path to player
        if (player != null)
        {
            Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));

            if (!pathRequested && (currentPath == null || currentPath.Count == 0))
            {
                currentTarget = playerPos;

                Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                RequestPath(currentPos, playerPos);
            }

            MoveUnit();
        }
    }

    const int pathRange = 50;

    void RequestPath(Vector2Int start, Vector2Int target)
    {
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

    void MoveUnit()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > 1.5f && currentPath != null && currentPath.Count > 0)
        {
            Vector2Int currentTarget = currentPath[pathIndex];

            Vector3 targetPos = new Vector3(currentTarget.x + .5f, .5f, currentTarget.y + .5f);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);


            if ((transform.position - targetPos).sqrMagnitude < 0.1f)
            {
                //transform.position = targetPos;
                pathIndex++;

                if (pathIndex >= currentPath.Count)
                {
                    currentPath.Clear();
                    pathIndex = 0;
                }
            }

        }
    }

    public void FollowPlayer(PlayerController thePlayer)
    {
        player = thePlayer;

        state = UnitState.Following;
    }

    void LateUpdate()
    {
        // Face the camera
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
    }

    public void MoveTo(Vector3 pos)
    {
        //targetPos = pos;

        state = UnitState.Moving;
    }

     
}
