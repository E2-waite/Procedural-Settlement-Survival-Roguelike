using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

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

            if ((currentPath == null || currentPath.Count == 0))
            {
                currentTarget = playerPos;

                Vector2Int currentPos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

                currentPath = UnitHandler.Instance.FindPath(currentPos, playerPos);
            }

            MoveUnit();
        }
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
