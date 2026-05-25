using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static Pathfinding;
using static UnityEngine.GraphicsBuffer;

public class FollowerUnit : PathAgent
{
    public float followDist = 1.5f;
    PlayerController player;
    private Camera cam;
    protected virtual void Start()
    {
        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);
    }

    // Set state to following, set target player, and request a path
    public virtual void StartFollowing(PlayerController thePlayer)
    {
        player = thePlayer;

        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));
        Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

        RequestPath(currentPos, playerPos, player.transform.position);
    }

    public virtual void Command(GridTile tile)
    {

    }

    protected virtual int GetState()
    {
        return 0;
    }

    protected virtual void Update()
    {
        HandleStates();

        if (GetState() == Consts.FOLLOWING_STATE)
        {
            // Continuously update path if following player
            if (player != null)
            {
                Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));

                if (!pathRequested && (currentPath == null || currentPath.Count == 0) && Vector3.Distance(transform.position, player.transform.position) > followDist)
                {
                    Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                    RequestPath(currentPos, playerPos, player.transform.position);
                }
            }

            FollowPath();
        }
    }

    protected virtual bool HandleStates()
    {
        return false;
    }
}
