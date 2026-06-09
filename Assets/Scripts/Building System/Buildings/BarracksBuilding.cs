using System.Collections;
using UnityEngine;

public class BarracksBuilding : ConvertBuilding
{
    private IUnitRole role = new FighterRole();
    protected override IUnitRole Role => role;
    public override bool SameType(IUnitRole unitRole) => unitRole is FighterRole;
}
