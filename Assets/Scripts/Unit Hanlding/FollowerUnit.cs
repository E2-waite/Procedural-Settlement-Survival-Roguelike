using System.Collections.Generic;
using System.ComponentModel;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using static Pathfinding;
using static UnityEngine.GraphicsBuffer;
using static WorkerUnit;

public class FollowerUnit : Unit
{
    public float followDist = 1.5f;
    PlayerController player;

    protected override void Start()
    {
        base.Start();

        UnitHandler.Instance.AddUnit(this);
    }

    protected override void Update()
    {
        base.Update();
    }

    #region States
    protected override void MoveState()
    {
        FollowPath();
    }

    // Follow player
    protected override void FollowState()
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
    #endregion
    #region HitHandling

    protected override void Die()
    {
        UnitHandler.Instance.RemoveUnit(this);
        base.Die();
    }

    #endregion

    #region Command
    public virtual void Command(GridTile tile)
    {
        // Move to tile if empty
        pathRequested = false;
        currentPath.Clear();

        SetState(State.Moving);
        RequestPath(GridPos(), tile.position, tile.worldPosition);
    }

    public virtual void Command(Unit unit)
    {
    }

    #endregion

    #region Target
    // Set state to following, set target player, and request a path
    public virtual void StartFollowing(PlayerController thePlayer)
    {
        player = thePlayer;

        SetState(State.Following);

        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));

        RequestPath(GridPos(), playerPos, player.transform.position);
    }
    #endregion
}
