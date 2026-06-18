using System.Security.Cryptography;
using UnityEngine;
public class WorkerRole : IUnitRole
{
    public enum State
    {
        Idle,
        Gather,
        Store,
        Build,
        Convert
    }
    public State state, lastState;
    Unit unit;
    private ResourceSystem resourceSystem; 
    private BuildingSystem buildingSystem;

    ResourceNode targetResource;
    public ResourceBuilding targetStore;
    Building targetBuilding;
    float interactInterval = 0.5f, interactTimer = 0;
    [SerializeField] public ResourceStorage storage = new ResourceStorage();

    public float interactDist = 1.5f;

    private int maxCapacity = 10;
    public IUnitRole New()
    {
        return new WorkerRole();
    }

    public void Init(GameContext context, Unit unit)
    {
        this.unit = unit;
        resourceSystem = context.resourceSystem;
        buildingSystem = context.buildingSystem;
        storage.max = maxCapacity;
        unit.UpdateObject(context.agentCatalog.worker);
        Debug.Log("Init worker");
    }

    public void Tick()
    {
        if (interactTimer > 0) interactTimer -= Time.deltaTime;
    }

    public bool InRange => Vector3.Distance(unit.transform.position, TargetPos()) < interactDist;

    // Returns the position of the current work target
    Vector3 TargetPos()
    {
        Vector3 pos = unit.transform.position;

        switch (state)
        {
            case State.Gather:
                if (targetResource != null)
                {
                    pos = targetResource.tile.worldPosition;
                }
                break;
            case State.Store:
                if (targetStore != null)
                {
                    pos = targetStore.transform.position;
                }
                break;
            case State.Build:
                if (targetBuilding != null)
                {
                    pos = targetBuilding.transform.position;
                }
                break;
        }

        return pos;
    }

    public void OnHit(float damage, Destructable source)
    {
        // Maybe flee?
    }

    // Called when unit reaches target position when in moving state
    public void OnReachedTarget()
    {
        SetState(State.Idle);
    }

    #region States
    // Sets the current work state
    void SetState(State state)
    {
        if (this.state != state)
        {
            lastState = this.state;
            this.state = state;
        }
    }

    // Handles state execution
    public void HandleStates()
    {
        switch (state)
        {
            case State.Gather:
                GatherState(); break;

            case State.Store:
                StoreState(); break;

            case State.Build:
                BuildState(); break;
        }
    }

    // GatherState: Gather resources when in range
    void GatherState()
    {
        if (InRange)
        {
            if (interactTimer <= 0)
                Gather();
        }
        else
        {
            unit.Movement.FollowPath();
        }
    }

    // StoreState : Stores resources when in range and find the next available resource
    void StoreState()
    {
        if (InRange)
        {
            Store();

            // After storing, immediately look for the next nearby resource to keep the worker busy.
            ResourceNode nextNode = resourceSystem.GetClosestNode(targetStore);
            if (nextNode == null && targetResource != null)
            {
                // If no resources in range of store, find closest node to current target resource
                if (targetResource.IsEmpty())
                    nextNode = resourceSystem.GetClosestNeighbour(targetResource);
                else
                    nextNode = targetResource;
            }

            if (nextNode != null)
            {
                Target(nextNode);
            }
            else
            {
                SetState(State.Idle);
                targetResource = null;
            }
        }
        else
        {
            unit.movement.FollowPath();
        }
    }

    // Build buildings when in range
    void BuildState()
    {
        if (InRange)
        {
            if (interactTimer <= 0)
            {
                if (Build())
                {
                    // Finished building
                    SetState(State.Idle);
                }
            }

        }
        else
        {
            unit.Movement.FollowPath();
        }
    }
    #endregion
    #region Commanding

    public bool Command(GridTile tile)
    {
        return false;
    }

    public bool Command(Agent agent)
    {
        return false;
    }

    public bool Command(Building building) 
    {
        if (!building.Built)
        {
            // If building isn't built, repair/build
            Target(building);
            return true;
        }
        else
        {
            // If building IS built, interact
            if (building is ResourceBuilding)
            {
                Target((ResourceBuilding)building);
                return true;
            }
        }
        return false;
    }
    #endregion
    #region Targeting
    public void Target(ResourceNode resource)
    {
        if (resource == null) return;

        // Resource targets always put the worker into gather mode and path to the node tile.
        targetResource = resource;
        SetState(State.Gather);

        unit.RequestPath(targetResource.tile.position, true);
    }

    // Target the passed resource building
    public void Target(ResourceBuilding store)
    {
        if (store != null)
        {
            if (storage.IsEmpty(store.type))
            {
                // If we don't have resources, just start gathering closest nodes
                ResourceNode closestResource = resourceSystem.GetClosestNode(store);
                Target(closestResource);
            }
            else
            {
                // Store resources if have some
                targetStore = store;
                SetState(State.Store);

                Vector2Int storePos = new Vector2Int((int)store.transform.position.x, (int)store.transform.position.z);

                unit.RequestPath(storePos);
                targetResource = null; // Don't return to gathering if we've commanded to store
            }
        }
    }

    // Target building to start building state
    public void Target(Building building)
    {
        if (building == null) return;

        targetBuilding = building;
        SetState(State.Build);
        unit.RequestPath(building.GridPos);
    }
    #endregion
    #region Actions
    // Checks if resources are at capacity and switch to storing if so
    public bool CheckCapacity()
    {
        if (storage.AtCapacity())
        {
            // Find closest resource store
            ResourceBuilding closestStore = buildingSystem.GetClosestStore(ResourceNode.Type.Wood, unit.GridPos);
            if (closestStore == null)
            {
                //SetState(State.Idle);
            }
            if (closestStore != null)
            {
                Target(closestStore);
            }

            return true;
        }
        return false;
    }
    public void Gather()
    {
        if (!CheckCapacity())
        {
            Debug.Log("Gathering " + targetResource.type.ToString());

            storage.Add(targetResource.type, targetResource.Gather(5));
            interactTimer = interactInterval;

            if (targetResource.IsEmpty())
            {
                ResourceNode neighbuoringNode = resourceSystem.GetClosestNeighbour(targetResource);

                if (neighbuoringNode != null)
                {
                    Target(neighbuoringNode);
                }
                else
                {
                    targetResource = null;
                }
            }

            CheckCapacity();
        }
    }

    bool Store()
    {
        if (targetStore != null)
        {
            targetStore.Store(storage.Get(targetStore.type));
            storage.Clear(targetStore.type);

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
