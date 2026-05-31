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
        movement.FollowPath();
    }

    // Follow player
    protected override void FollowState()
    {
        // Continuously update path if following player
        if (player != null)
        {
            Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));

            if (!pathRequested && !movement.HasPath() && Vector3.Distance(transform.position, player.transform.position) > followDist)
            {
                RequestPath(playerPos, player.transform.position);
            }
        }

        movement.FollowPath();
    }
    #endregion

    #region Taking Damage
    protected override void OnDeathStart()
    {
        UnitHandler.Instance.RemoveUnit(this);
    }

    #endregion

    #region Command
    public virtual void Command(GridTile tile)
    {
        // Move to tile if empty
        pathRequested = false;
        movement.ClearPath();

        SetState(State.Moving);
        RequestPath(tile.position, tile.worldPosition);
    }

    public virtual void Command(Unit unit)
    {
    }
    #endregion

    public override List<Unit> GetNearbyFriendly()
    {
        if (chunk != null)
        {
            return chunk.GetFollowers();
        }

        return null;
    }

    #region Targeting
    // Set state to following, set target player, and request a path
    public virtual void StartFollowing(PlayerController thePlayer)
    {
        player = thePlayer;

        SetState(State.Following);

        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));

        RequestPath(playerPos, player.transform.position);
    }
    #endregion
}
