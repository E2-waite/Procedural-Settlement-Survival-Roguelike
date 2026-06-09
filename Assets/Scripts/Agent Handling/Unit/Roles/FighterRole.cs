using System.Collections.Generic;
using UnityEngine;

public class FighterRole : IUnitRole
{
    [SerializeField] public AgentCombat combat = new AgentCombat();
    public AgentCombat Combat => combat;

    private FighterTargetting targetting = new FighterTargetting();
    public FighterTargetting Targetting => targetting;

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
        Targetting?.Init(unit);
        Combat?.Init(unit, Targetting);
        unit.UpdateObject(context.agentCatalog.fighter);
        Debug.Log("Init Fighter");
    }

    public void Tick()
    {
        Targetting?.Tick();
        Combat?.Tick();
    }

    public void OnHit(float damage, Destructable source)
    {
        // Target hit source
        Targetting.AddTarget(source, damage);
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
            Targetting.Target((Enemy)agent);
            unit.RequestPath(Targetting.TargetPos(), agent.transform.position);
            return true;
        }

        return false;
    }

    public bool Command(Building building)
    {
        return false;
    }
    #endregion
    #region Targetting

    #endregion
}
