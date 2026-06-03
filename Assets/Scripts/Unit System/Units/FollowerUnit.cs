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
    Player player;
    public bool Following => state == State.Following;
    private bool commanding = false; // This unit is being commanded
    public bool Commanding => commanding;

    protected override void Start()
    {
        base.Start();

        UnitSystem.Instance.AddUnit(this);
    }

    protected override void Update()
    {
        base.Update();
    }

    #region States

    // Moves to target position
    protected override void MoveState()
    {
        movement.FollowPath();

        if (movement.HasPath && movement.TargetReached)
        {
            TargetTileReached();
        }
    }

    protected virtual void TargetTileReached() { }

    // Follow player
    protected override void FollowState()
    {
        // Continuously update path
        if (player != null)
        {
            Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));

            if (!pathRequested && (movement.TargetReached || !movement.HasPath) && Vector3.Distance(transform.position, player.transform.position) > followDist)
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
        UnitSystem.Instance.RemoveUnit(this);
    }

    #endregion

    #region Command

    // Commands to move to tile
    public virtual void Command(GridTile tile)
    {
        // Move to tile if empty
        RequestPath(tile);
        SetState(State.Moving);
    }

    public virtual void Command(Unit unit)
    {
        // No longer following player
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
    public void SetNearby()
    {
        if (!commanding)
        {
            markerSprite.enabled = true;
            markerSprite.color = Color.white;
        }
    }

    public void ClearNearby()
    {
        if (!commanding)
        {
            markerSprite.enabled = false;
        }
    }

    // Set state to following, set target player, and request a path
    public virtual void StartCommanding(Player thePlayer)
    {
        commanding = true;
        player = thePlayer;
        markerSprite.enabled = true;
        markerSprite.color = Color.green;
    }

    // Stops following the player
    public virtual void StopCommanding()
    {
        if (state == State.Following)
            SetIdle();

        commanding = false;
        player = null;
        markerSprite.enabled = false;
    }

    public virtual void StartFollowing()
    {
        if (player != null)
        {
            SetState(State.Following);
            Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));
            RequestPath(playerPos, player.transform.position);
        }
    }
    #endregion
}
