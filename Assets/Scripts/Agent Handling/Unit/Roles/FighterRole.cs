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

    public IUnitRole New()
    {
        return new FighterRole();
    }
    public void Init(GameContext context, Unit unit)
    {
        this.unit = unit;
        Targeting?.Init(unit);
        Combat?.Init(unit, Targeting);
        unit.UpdateObject(context.agentCatalog.fighter);
        Debug.Log("Init Fighter");
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
    public bool Command(GridTile tile)
    {
        Combat.DefendTile(tile);
        return false;
    }

    // Commands fighter to target agent
    public bool Command(Agent agent)
    {
        if (agent is Enemy)
        {
            Targeting.Target(agent);
            unit.RequestPath(Targeting.TargetPos(), agent.transform.position);
            return true;
        }

        return false;
    }

    public bool Command(Building building)
    {
        if (building.Owner == Faction.Enemy) // Target enemy buildings
        {
            Targeting.Target(building);
            unit.RequestPath(Targeting.TargetPos(), building.transform.position);
            return true;
        }
        return false;
    }
    #endregion
    #region Targeting

    #endregion
}
