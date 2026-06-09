using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Agent
{
    public enum State
    {
        Idle,
        Moving,
        Following,
        Combat,
        Work
    }
    [SerializeField] public AgentCombat combat = new AgentCombat();

    public override AgentCombat Combat => combat;
    public float followDist = 1.5f;
    Player player;
    public bool Following => state == State.Following;
    private bool commanding = false; // This unit is being commanded
    public bool Commanding => commanding;
    private UnitSystem unitSystem;
    public override TargetType Type => TargetType.Agent;
    public SpriteRenderer markerSprite;
    public State state, lastState;
    public float swarmRadius = .5f;
    private Camera cam;
    protected float scanInterval = 1.0f, scanTimer = 0; // Timer for tracking when to next scan for nearby friendly units
    protected FireSystem fireSystem;
    protected PathfindingHandler pathfinding;

    public override void Init(GameContext context)
    {
        base.Init(context);
        World world = context.world;
        fireSystem = context.fireSystem;
        pathfinding = context.pathfinding;

        health.Fill();

        state = State.Idle;
        lastState = State.Idle;

        unitSystem = context.unitSystem;
        unitSystem.AddUnit(this);
    }

    protected virtual void Start()
    {
        // Face the camera

    }

    protected override void Update()
    {
        if (IsDead) return; // Dead
        base.Update();
    }

    #region States
    // Generic SetState function for setting units' state to generic state (idle, moving, following and attacking)
    public virtual void SetState(State newState)
    {
        if (state != newState)
        {
            lastState = state;
            state = newState;
        }
    }

    // Generic GetState function for getting units' state as an int
    protected virtual State GetState()
    {
        return state;
    }

    public void SetIdle()
    {
        SetState(State.Idle);
    }

    public void StartCombat()
    {
        SetState(State.Combat);
    }

    protected override void HandleStates()
    {
        switch(state)
        {
            case State.Idle:
                IdleState(); break;

            case State.Moving:
                MovingState(); break;

            case State.Following:
                FollowState(); break;

            case State.Combat:
                CombatState(); break;

            case State.Work:
                WorkingState(); break;
        }
    }

    protected virtual void IdleState()
    {
        // Do nothing
    }

    // Moves to target position
    protected virtual void MovingState()
    {
        movement.FollowPath();

        if (movement.HasPath && movement.TargetReached)
        {
            TargetTileReached();
        }
    }

    // Follow player
    protected virtual void FollowState()
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

    protected virtual void CombatState()
    {
        if (Combat != null)
        {
            Combat.UpdateState();
            Combat.ExecuteState();
        }
    }

    protected virtual void WorkingState()
    {

    }
    #endregion

    #region Taking Damage
    protected override void OnDeathStart()
    {
        base.OnDeathStart();
        unitSystem.RemoveUnit(this);
    }

    // Handles receiving hits from another agent. Returns true if target is dead
    public override bool Hit(float damage, Destructable source)
    {
        bool died = base.Hit(damage, source);

        if (!died && Combat != null)
        {
            Targetting.AddTarget(source, 5);
        }

        return died;
    }

    #endregion

    #region Commanding

    // Commands to move to tile
    public virtual void Command(GridTile tile)
    {
        // Move to tile if empty
        RequestPath(tile);
        SetState(State.Moving);
    }

    public virtual void Command(Agent agent)
    {
        // No longer following player
    }

    public virtual void Command(Building building) { }
    #endregion

    #region Targeting
    // Targets the passed agent and sets state to attacking
    protected override void TargetAgent(Agent agent)
    {
        if (Combat != null && agent != null)
        {
            Targetting.Target(agent);

            SetState(State.Combat);

            RequestPath(Targetting.TargetPos(), agent.transform.position);
        }
    }
    #endregion

    #region Agent Detection
    public override List<Agent> GetNearbyFriendly()
    {
        if (chunk != null)
        {
            return chunk.GetUnits();
        }

        return null;
    }
    // TODO: get nearby when the chunk's units change, rather than continuously every second
    #endregion

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
}
