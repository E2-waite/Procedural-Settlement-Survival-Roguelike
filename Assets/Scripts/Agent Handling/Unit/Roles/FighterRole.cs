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
        SearchForEnemies();

        Targetting?.Tick();
        Combat?.Tick();
    }

    public void OnHit(float damage, Destructable source)
    {
        Targetting.AddTarget(source, damage);
    }

    public void HandleStates()
    {

    }

    public void OnReachedTarget()
    {
        Combat.StartDefending();
    }

    #region Commanding
    // Commands to move to tile
    public bool Command(GridTile tile)
    {
        if (tile == null) return false;
        Combat.DefendTile(tile);
        return false;
    }

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
    private void Target(Enemy enemy)
    {

    }

    void SearchForEnemies()
    {
        // Find hostile units
        List<Agent> enemies = unit.GetNearbyHostile();

        foreach (Agent enemy in enemies)
        {
            Targetting.AddTarget(enemy, 10f);
        }

        // Add nearby hostile targets to target candidates
        //Combat.AddTargets(hostile);
    }
    #endregion


}
