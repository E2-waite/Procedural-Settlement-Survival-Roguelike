using System.Collections;
using UnityEngine;
using static GlobalDefs;
public class ArcheryRangeBuilding : ConvertBuilding
{
    private IUnitRole role = new FighterRole(CombatType.Ranged);
    protected override IUnitRole Role => role;
    public override bool SameType(IUnitRole unitRole) => unitRole is FighterRole;
}
