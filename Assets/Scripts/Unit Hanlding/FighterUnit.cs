using System.Collections.Generic;
using UnityEngine;

public class FighterUnit : FollowerUnit
{
    protected override void Start()
    {
        base.Start();
        combat.SetUnit(this);
    }

    protected override void Update()
    {
        base.Update();
    }

    #region Taking Damage
    public override bool Hit(float damage, Unit source)
    {
        if (base.Hit(damage, source)) return true;

        if (source is EnemyUnit)
        {
            // Update target?
            TargetUnit(source);
        }

        return false;
    }

    #endregion

    #region Commanding

    // Command to interact with tile
    public override void Command(GridTile tile)
    {
        if (tile == null) return;
        bool handled = false;

        if (!handled)
            base.Command(tile);
    }

    // Command to interact with unit
    public override void Command(Unit unit)
    {
        if (unit == null || unit == this) return;

        if (unit is EnemyUnit)
        {
            TargetUnit(unit);
        }
    }

    #endregion

    #region Detecting Units
    // Gets nearby follower units for targetting
    protected override List<Unit> GetNearbyUnits()
    {
        if (chunk != null)
        {
            return chunk.GetEnemies();
        }

        return null;
    }

    #endregion
}
