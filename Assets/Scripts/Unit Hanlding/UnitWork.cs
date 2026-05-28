using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static WorkerUnit;

// Class for handling worker unit interactions
[System.Serializable]
public class UnitWork
{
    public enum WorkState
    {
        None,
        Gathering,
        Storing,
        Building
    }
    public WorkState state, lastState;
    public float interactDist = 1.5f;
    float interactInterval = 0.5f, interactTimer = 0;

    WorkerUnit unit;

    ResourceNode targetResource;
    public ResourceBuilding targetStore;
    Building targetBuilding;

    public void SetUnit(WorkerUnit unit)
    {
        this.unit = unit;
    }

    public void Update()
    {
        if (interactTimer > 0) interactTimer -= Time.deltaTime;
    }

    Vector3 TargetPos()
    {
        Vector3 pos = unit.transform.position;

        switch (state)
        {
            case WorkState.Gathering:
                if (targetResource != null)
                {
                    pos = targetResource.tile.worldPosition;
                }
                break;
            case WorkState.Storing:
                if (targetStore != null)
                {
                    pos = targetStore.transform.position;
                }
                break;
            case WorkState.Building:
                if (targetBuilding != null)
                {
                    pos = targetBuilding.transform.position;
                }
                break;
        }

        return pos;
    }

    public bool InRange()
    {
        return Vector3.Distance(unit.transform.position, TargetPos()) < interactDist;
    }

    void MoveToTarget()
    {

    }

    #region States
    void SetState(WorkState state)
    {
        lastState = this.state;
        this.state = state;
    }

    // Executes the current combat state
    public void ExecuteState()
    {
        switch (state)
        {
            case WorkState.Gathering:
                GatherState(); break;

            case WorkState.Storing:
                StoreState(); break;

            case WorkState.Building:
                BuildState(); break;
        }
    }

    void GatherState()
    {
        if (InRange())
        {
            if (interactTimer <= 0)
                Gather();
        }
        else
        {
            unit.FollowPath();
        }
    }

    void StoreState()
    {
        if (InRange())
        {
            Store();

            // Find closest resource to store
            ResourceNode nextNode = ResourceHandler.Instance.GetClosestNode(targetStore);

            if (nextNode == null && targetResource != null)
            {
                // If no resources in range of store, find closest node to current target resource
                if (targetResource.IsEmpty())
                    nextNode = ResourceHandler.Instance.GetClosestNeighbour(targetResource);
                else
                    nextNode = targetResource;
            }

            if (nextNode != null)
            {
                SetTarget(nextNode);
            }
            else
            {
                SetState(WorkState.None);
                unit.SetIdle();
                targetResource = null;
            }
        }
        else
        {
            unit.FollowPath();
        }
    }

    void BuildState()
    {
        if (InRange())
        {
            if (interactTimer <= 0)
            {
                if (Build())
                {
                    // Finished building
                    SetState(WorkState.None);
                    unit.SetIdle();
                }
            }
                
        }
        else
        {
            unit.FollowPath();
        }
    }
    #endregion
    #region Targeting
    public void SetTarget(ResourceNode resource)
    {
        if (resource == null) return;

        targetResource = resource;
        SetState(WorkState.Gathering);

        unit.RequestPath(targetResource.tile.position, targetResource.tile.worldPosition);
    }

    public void SetTarget(ResourceBuilding store)
    {
        if (store != null)
        {
            if (unit.storage.IsEmpty(store.type))
            {
                // If we don't have resources, just start gathering closest nodes
                ResourceNode closestResource = ResourceHandler.Instance.GetClosestNode(store);
                SetTarget(closestResource);
            }
            else
            {
                // Store resources if have some
                targetStore = store;
                SetState(WorkState.Storing);

                Vector2Int storePos = new Vector2Int((int)store.transform.position.x, (int)store.transform.position.z);

                unit.RequestPath(storePos, store.transform.position);
                targetResource = null; // Don't return to gathering if we've commanded to store
            }
        }
    }

    public void SetTarget(Building building)
    {
        if (building == null) return;

        targetBuilding = building;
        SetState(WorkState.Building);

        Vector2Int buildingPos = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.z);

        unit.RequestPath(buildingPos, building.transform.position);
    }
    #endregion

    #region Actions
    public void Gather()
    {
        if (!unit.CheckCapacity())
        {
            Debug.Log("Gathering " + targetResource.type.ToString());

            unit.storage.Add(targetResource.type, targetResource.Gather(5));
            interactTimer = interactInterval;

            if (targetResource.IsEmpty())
            {
                ResourceNode neighbuoringNode = ResourceHandler.Instance.GetClosestNeighbour(targetResource);

                if (neighbuoringNode != null)
                {
                    SetTarget(neighbuoringNode);
                }
                else
                {
                    targetResource = null;
                }
            }

            unit.CheckCapacity();
        }
    }

    bool Store()
    {
        if (targetStore != null)
        {
            targetStore.Store(unit.storage.Get(targetStore.type));
            unit.storage.Clear(targetStore.type);

            // Find next node
            return true;
        }
        return false;
    }

    bool Build()
    {
        if (targetBuilding != null)
        {
            bool finished = targetBuilding.Build(10);
            interactTimer = interactInterval;
            return finished;
        }
        return false;
    }
    #endregion
}
