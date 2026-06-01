using Unity.VisualScripting;
using UnityEngine;

public class WorkerUnit : FollowerUnit
{
    [SerializeField] public ResourceStorage storage = new ResourceStorage();
    [SerializeField] UnitWork work = new UnitWork();

    protected override void Start()
    {
        base.Start();
        work.SetUnit(this);
    }

    protected override void Update()
    {
        base.Update();
        work.Update();
    }

    #region States

    protected override void WorkingState()
    {
        work.ExecuteState();
    }

    #endregion

    #region Commanding

    public override void Command(GridTile tile)
    {
        if (tile == null) return;

        bool handled = false;

        if (tile.HasBuilding())
        {
            // Interact with building if tile has one
            Building building = tile.Building();

            if (!building.Built())
            {
                // If building isn't built, repair/build
                TargetBuilding(building);
            }
            else
            {
                // If building IS built, interact
                if (building is ResourceBuilding)
                {
                    TargetStore((ResourceBuilding)building);
                }
                else if (building is BarracksBuilding)
                {
                    TargetBuilding(building);
                }
            }

            handled = true;
        }

        if (!handled)
            base.Command(tile);
    }

    #endregion

    #region Targeting Objects (Resources, stores and buildings)
    public void TargetResource(ResourceNode resource)
    {
        SetState(State.Work);
        work.SetTarget(resource);
    }

    void TargetStore(ResourceBuilding store)
    {
        SetState(State.Work);
        work.SetTarget(store);
    }

    void TargetBuilding(Building building)
    {
        SetState(State.Work);
        work.SetTarget(building);
    }
    #endregion

    #region Worker Actions (Gathering, storing and building)


    // Checks if resources are at capacity and switch to storing if so
    public bool CheckCapacity()
    {
        if (storage.AtCapacity())
        {
            // Find closest resource store
            ResourceBuilding closestStore = BuildingManager.Instance.GetClosestStore(ResourceNode.Type.Wood, GridPos());
            if (closestStore == null)
            {
                SetState(State.Idle);
            }
            if (closestStore != null)
            {
                TargetStore(closestStore);
            }

            return true;
        }
        return false;
    }

    #endregion
}
