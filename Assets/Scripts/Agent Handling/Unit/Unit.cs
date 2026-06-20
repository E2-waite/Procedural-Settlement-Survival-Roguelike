using System.Collections.Generic;
using UnityEngine;
using static GlobalDefs;
using static Calculations;
public class Unit : Agent
{
    public enum State
    {
        Idle,
        Working,
        Moving,
        Following,
        Converting
    }

    public float followDist = 1.5f;
    Player player;
    public bool Following => state == State.Following;
    private Faction faction = Faction.Neutral;
    public Faction Faction => faction;
    private bool commanding = false; // This unit is being commanded
    public bool Commanding => commanding;
    private UnitSystem unitSystem;
    public override TargetType Type => TargetType.Agent;
    public SpriteRenderer markerSprite;
    public State state, lastState;
    private Camera cam;
    protected float scanInterval = 1.0f, scanTimer = 0; // Timer for tracking when to next scan for nearby friendly units
    protected FireSystem fireSystem;

    private IUnitRole role;
    public IUnitRole Role => role;
    private ConvertBuilding convertBuilding = null;
    private GameContext gameContext;
    private UnitSpawnerBuilding spawner;
    protected override Color HighlightColor => Color.green;

    public override void Init(GameContext context)
    {
        base.Init(context);
        World world = context.world;
        fireSystem = context.fireSystem;
        health.Fill();

        state = State.Idle;
        lastState = State.Idle;


        unitSystem = context.unitSystem;
        //unitSystem.AddUnit(this);

        role?.Init(context, this);
        gameContext = context;
        UpdateObject(context.agentCatalog.unit);
    }

    public void Init(GameContext context, UnitSpawnerBuilding spawner)
    {
        this.spawner = spawner;
        Init(context);
    }
    protected override void Update()
    {
        if (IsDead) return; // Dead

        base.Update();

        if (squad != null && state != State.Following)
        {
            Sprite?.SetDirection(-squad.FacingDir);
        }
    }

    public bool Recruit()
    {
        if (unitSystem.AddUnit(this))
        {
            faction = Faction.Unit;
            Debug.Log("Recruited " + name);
            return true;
        }
        else
        {
            Debug.Log("Failed to recruit " + name);
        }

        return false;
    }

    #region Roles
    // Update and init the current role on convert
    public void SetRole(IUnitRole newRole)
    {
        role = newRole;
        role.Init(gameContext, this);
    }

    // Updates the agent object on convert or init
    public void UpdateObject(AgentObject agentObject)
    {
        Sprite.Init(body.transform, outline.transform, agentObject);
        roleType = agentObject.AgentType;
    }

    #endregion
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

    public void SetWorking()
    {
        SetState(State.Working);
    }

    protected override void HandleStates()
    {
        switch(state)
        {
            case State.Working:
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
            if (role == null)
            {
                SetState(State.Idle);
            }
            else
            {
                SetState(State.Working);
            }
        }
    }

    Vector3 FollowPos()
    {
        if (player == null) return Vector3.zero;

        Vector3 formationPos = Squad == null ? Vector3.zero : Squad.FollowFormationPos(this);
        if (squad != null)
        {
            formationPos = squad.FacingRot * formationPos;
        }

        return player.transform.position + formationPos;
    }

    // Continuously follows the player
    protected virtual void FollowState()
    {
        // Continuously update path
        if (player != null)
        {
            Vector3 followPos = FollowPos();
            movement.SetTargetPos(followPos);

            if (!pathRequested && movement.TargetChanged)
            {
                Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(followPos.x), Mathf.FloorToInt(followPos.z));
                GridTile gridTile = grid.GetTile(gridPos);
                if (gridTile == null) return;

                RequestPath(gridTile, followPos);
            }

            movement.FollowPath();
        }
    }

    // Update role
    protected virtual void WorkingState()
    {
        role?.Tick();
        role?.HandleStates();
    }


    // Moves towards convert building and convert unit role when in range
    protected virtual void ConvertState()
    {
        movement.FollowPath();

        if (convertBuilding == null) return;

        float dist = Vector3.Distance(transform.position, convertBuilding.transform.position);
        dist -= convertBuilding.transform.localScale.x;
        if (movement.HasPath && movement.TargetReached || dist <= 1f)
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
        unitSystem.RemoveUnit(this); // Removes this unit on death
    }

    // Handles receiving hits from another agent. Returns true if target is dead
    public override bool Hit(Destructable source, float damage, Vector3 dir)
    {
        bool died = base.Hit(source, damage, dir);

        if (!died)
        {
            role?.OnHit(damage, source); // Trigger hit event in current role
        }

        return died;
    }

    #endregion
    #region Commanding

    // Start commanding this unit
    public virtual void StartCommanding(Player thePlayer)
    {
        commanding = true;
        player = thePlayer;
        markerSprite.enabled = true;
        if (HasSquad)
            markerSprite.color = Squad.color;
        else
            markerSprite.color = Color.green;
        spawner?.RemoveAgent(this);
    }

    // Stops commanding this unit
    public virtual void StopCommanding()
    {
        if (state == State.Following)
            SetWorking();

        commanding = false;
        player = null;
        markerSprite.enabled = false;
    }

    // Commands unit to move to the passed tile
    public virtual void Command(GridTile tile, Vector3 pos)
    {
        role?.Command(tile, pos);

        // Move to tile if empty and role didn't consume command
        RequestPath(pos);
        SetState(State.Moving);
    }

    // Commands unit to interact with an agent
    public virtual void Command(Agent agent)
    {
        if (role != null && role.Command(agent))
        {
            SetState(State.Working);
        }
    }

    // Commands unit to interact with a building
    public virtual void Command(Building building) 
    {
        if (building is ConvertBuilding)
        {
            ConvertBuilding convertBuilding = (ConvertBuilding)building;

            if (!convertBuilding.SameType(role)) // Only convert if building is not of the same role type
                StartConverting((ConvertBuilding)building);
        }
        else if (role != null && role.Command(building))
        {
            SetState(State.Working);
        }
    }

    // Sets convert state and requests path
    private void StartConverting(ConvertBuilding building)
    {
        convertBuilding = building;
        SetState(State.Converting);
        RequestPath(building.Tile);
    }

    // Sets follow state and requests path
    public virtual void StartFollowing()
    {
        if (player != null)
        {
            SetState(State.Following);
            Vector3 followPos = FollowPos();
            Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(followPos.x), Mathf.FloorToInt(followPos.z));

            RequestPath(grid.GetTile(gridPos), followPos);
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
}
