using System.Collections;
using UnityEngine;
using static GlobalDefs;
public class BarracksBuilding : ConvertBuilding
{
    private IUnitRole role = new FighterRole(CombatType.Melee);
    protected override IUnitRole Role => role;
    public override bool SameType(IUnitRole unitRole) => unitRole is FighterRole;
}
