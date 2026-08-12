using UnityEngine;

public class HouseBuilding : Building
{
    [SerializeField] private int capacity = 5;
    UnitSystem unitSystem;
    public override void Init(GameContext context)
    {
        base.Init(context);

        unitSystem = context.unitSystem;
    }

    protected override void FinishBuilding()
    {
        base.FinishBuilding();
        unitSystem.IncreaseCapacity(capacity);
    }
}
