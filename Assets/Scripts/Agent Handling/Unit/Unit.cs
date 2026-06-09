using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Unit : Agent
{
    public enum State
    {
        None,
        Moving,
        Following,
        Converting
    }

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

    private IUnitRole role;
    public IUnitRole Role => role;
    private ConvertBuilding convertBuilding = null;
    private GameContext gameContext;
    public override void Init(GameContext context)
    {
        base.Init(context);
        World world = context.world;
        fireSystem = context.fireSystem;
        health.Fill();

        state = State.None;
        lastState = State.None;

        unitSystem = context.unitSystem;
        unitSystem.AddUnit(this);

        role?.Init(context, this);
        gameContext = context;
        UpdateObject(context.agentCatalog.unit);
    }

    protected override void Update()
    {
        if (IsDead) return; // Dead

        base.Update();

        role?.Tick();
        if (state == State.None) role?.HandleStates();
    }

    public void SetRole(IUnitRole newRole)
    {
        role = newRole;
        role.Init(gameContext, this);
    }

    public void UpdateObject(AgentObject agentObject)
    {
        Sprite.Init(spriteRend, agentObject);
    }

    #region States
    // Generic SetState function for setting units' state to generic state (Working, moving, following and attacking)
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

    public void SetWorking()
    {
        SetState(State.None);
    }

    public void StartCombat()
    {
        //SetState(State.Combat);
    }

    protected override void HandleStates()
    {
        switch(state)
        {
            case State.None:
                WorkingState(); break;

            case State.Moving:
                MovingState(); break;

            case State.Following:
                FollowState(); break;

            case State.Converting:
                ConvertState(); break;
        }
    }

    // Moves to target position
    protected virtual void MovingState()
    {
        movement.FollowPath();

        if (movement.HasPath && movement.TargetReached)
        {
            SetState(State.None);
            role?.OnReachedTarget();
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

    protected virtual void WorkingState()
    {

    }

    protected virtual void ConvertState()
    {
        movement.FollowPath();

        if (movement.HasPath && movement.TargetReached)
        {
            if (convertBuilding != null)
            {
                convertBuilding.Convert(this);
                convertBuilding = null;
            }
        }
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

        if (!died)
        {
            role?.OnHit(damage, source);
        }

        return died;
    }

    #endregion

    #region Commanding

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
            SetWorking();

        commanding = false;
        player = null;
        markerSprite.enabled = false;
    }

    // Commands to move to tile
    public virtual void Command(GridTile tile)
    {
        if (role != null && role.Command(tile))
        {
            SetState(State.None);
        }
        else
        {
            // Move to tile if empty
            RequestPath(tile);
            SetState(State.Moving);
        }

    }

    public virtual void Command(Agent agent)
    {
        if (role != null && role.Command(agent))
        {
            SetState(State.None);
        }
    }

    public virtual void Command(Building building) 
    {
        if (building is ConvertBuilding)
        {
            ConvertBuilding convertBuilding = (ConvertBuilding)building;

            if (!convertBuilding.SameType(role))
                StartConverting((ConvertBuilding)building);
        }
        else if (role != null && role.Command(building))
        {
            SetState(State.None);
        }
    }

    private void StartConverting(ConvertBuilding building)
    {
        SetState(State.Converting);
        convertBuilding = (ConvertBuilding)building;
        RequestPath(building.Tile);
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


    #region Agent Detection
    public override List<Agent> GetNearbyFriendly()
    {
        if (chunk != null)
        {
            return chunk.GetUnits();
        }

        return null;
    }   
    
    public override List<Agent> GetNearbyHostile()
    {
        if (chunk != null)
        {
            return chunk.GetEnemies();
        }

        return null;
    }
    // TODO: get nearby when the chunk's units change, rather than continuously every second
    #endregion



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
