using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static Pathfinding;
using static UnityEngine.GraphicsBuffer;

public class FollowerUnit : PathAgent
{
    public enum UnitState
    {
        Idle,
        Moving,
        Following
    }

    public UnitState state;
    PlayerController player;

    private Camera cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Face the camera
        cam = Camera.main;
        Vector3 forward = cam.transform.forward;
        forward.Normalize();
        transform.rotation = Quaternion.LookRotation(forward);

        state = UnitState.Idle;
    }

    // Update is called once per frame
    void Update()
    {

        if (state == UnitState.Moving && !HasPath())
        {
            // If we are moving, but don't have a path become idle
            state = UnitState.Idle;
        }
        else if (state == UnitState.Following && !pathRequested && !HasPath())
        {
            if (player != null && Vector3.Distance(transform.position, player.transform.position) > reachedThresh)
            {
                // If we're following and don't have a path (and haven't already requested one) and we are not within the threshold, request a new path
                Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));
                Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                RequestPath(currentPos, playerPos, player.transform.position);
            }
        }

        if (state == UnitState.Following || state == UnitState.Moving)
            FollowPath();
    }

    // Set state to following, set target player, and request a path
    public void FollowPlayer(PlayerController thePlayer)
    {
        player = thePlayer;

        state = UnitState.Following;

        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));
        Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

        RequestPath(currentPos, playerPos, player.transform.position);
    }

    // Set state to moving, set target 'move to' postion, and request a path
    public void MoveTo(Vector3 pos)
    {
        pathRequested = false;
        currentPath.Clear();
        state = UnitState.Moving;

        Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
        Vector2Int targetPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.z));

        RequestPath(currentPos, targetPos, pos);
    }
}
