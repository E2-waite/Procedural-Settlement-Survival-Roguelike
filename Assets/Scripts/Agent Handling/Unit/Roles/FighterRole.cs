using System.Collections.Generic;
using UnityEngine;
using static GlobalDefs;
public class FighterRole : IUnitRole
{
    [SerializeField] public AgentCombat combat = new AgentCombat();
    public AgentCombat Combat => combat;

    private FighterTargetting targetting = new FighterTargetting();
    public FighterTargetting Targeting => targetting;

    private Unit unit;
    private AgentObject agentObject;
    public AgentObject AgentObject => agentObject;
    private CombatType combatType;

    public FighterRole(CombatType combatType)
    {
        this.combatType = combatType;
    }

    public IUnitRole New()
    {
        return new FighterRole(combatType);
    }

    public void Init(GameContext context, Unit unit)
    {
        this.unit = unit;
        Targeting?.Init(unit);
        Combat?.Init(unit, Targeting, combatType);

        AgentObject agentObject;
        if (combatType == CombatType.Melee)
        {
            Debug.Log("Init Melee Fighter");

            agentObject = context.agentCatalog.fighter;
        }
        else
        {
            Debug.Log("Init Ranged Fighter");

            agentObject = context.agentCatalog.ranger;
        }
        unit.UpdateObject(agentObject);
    }

    public void Tick()
    {
        Targeting?.Tick();
        Combat?.Tick();
    }

    public void OnHit(float damage, Destructable source)
    {
        // Target hit source
        Targeting.AddTarget(source, damage);
    }

    public void HandleStates()
    {

    }

    public void OnReachedTarget()
    {
        // Start defending if target reached when in moving state
        //Combat.StartDefending();
    }

    #region Commanding
    // Commands fighter to defend a tile (doesn't consume command)
    public bool Command(GridTile tile, Vector3 pos)
    {
        Combat.SetFormationPos(pos);
        return false;
    }

    // Commands fighter to target agent
    public bool Command(Agent agent)
    {
        if (agent is Enemy)
        {
            Targeting.Target(agent);
            unit.RequestPath(Targeting.TargetPos());
            return true;
        }

        return false;
    }

    public bool Command(Building building)
    {
        if (building.Owner == Faction.Enemy) // Target enemy buildings
        {
            Targeting.Target(building);
            unit.RequestPath(Targeting.TargetPos());
            return true;
        }
        return false;
    }
    #endregion
    #region Targeting

    #endregion
}
