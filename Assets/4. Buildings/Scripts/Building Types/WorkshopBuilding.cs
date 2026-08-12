using System.Collections;
using UnityEngine;

public class WorkshopBuilding : ConvertBuilding
{
    private IUnitRole role = new WorkerRole();
    protected override IUnitRole Role => role;
    public override bool SameType(IUnitRole unitRole) => unitRole is WorkerRole;
}
