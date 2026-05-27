using Unity.VisualScripting;
using UnityEngine;

public class WorkerUnit : FollowerUnit
{
    public enum WorkState
    {
        None,
        Gathering,
        Storing,
        Building
    }

    [SerializeField] ResourceStorage storage = new ResourceStorage();

    WorkState workState, lastWorkState;

    ResourceNode targetResource;
    public ResourceBuilding targetStore;
    Building targetBuilding;

    float interactInterval = 0.5f, interactTimer = 0;

    void SetWorkState(WorkState state)
    {
        lastWorkState = workState;
        workState = state;
    }

    protected override void Start()
    {
        base.Start();

        workState = WorkState.None;
        lastWorkState = WorkState.None;
    }

    protected override void Update()
    {
        if (interactTimer > 0) interactTimer -= Time.deltaTime;
        base.Update();
    }

    #region States

    protected override void WorkingState()
    {
        switch (workState)
        {
            case WorkState.Gathering:
                GatherState(); break;

            case WorkState.Storing:
                StoreState(); break;

            case WorkState.Building:
                BuildState(); break;
        }
    }

    protected void GatherState()
    {
        if (targetResource == null)
        {
            SetState(State.Idle);
        }
        else
        {
            float dist = Vector3.Distance(transform.position, targetResource.worldPosition);

            // Move towards resource if not in range
            if (dist > 1.25f)
            {
                FollowPath();
            }
            else
            {
                if (interactTimer <= 0)
                {
                    // Gather if in range and timer has finished
                    Gather();
                }
            }
        }
    }

    protected void StoreState()
    {
        if (targetStore == null)
        {
            SetState(State.Idle);
        }
        else
        {
            float dist = Vector3.Distance(transform.position, targetStore.transform.position);

            // Move towards resource if not in range
            if (dist > 1.25f)
            {
                FollowPath();
            }
            else
            {
                Store(); // Store resources if in range

                // TODO: move to storeing next resource type if available

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
                    TargetResource(nextNode);
                }
                else
                {
                    targetResource = null;
                }
            }
        }
    }

    protected void BuildState()
    {
        float dist = Vector3.Distance(transform.position, targetBuilding.transform.position);

        // Move towards resource if not in range
        if (dist > 1.25f)
        {
            FollowPath();
        }
        else
        {
            if (interactTimer <= 0)
            {
                // Gather if in range and timer has finished
                if (Build())
                {
                    targetBuilding = null;
                    SetState(State.Idle);
                }
            }
        }
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
            Building building = tile.GetBuilding();

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
            }

            handled = true;
        }

        if (!handled)
            base.Command(tile);
    }

    #endregion

    #region Targeting Objects (Resources, stores and buildings)
    void TargetResource(ResourceNode resource)
    {
        if (resource != null && !resource.IsEmpty())
        {
            targetResource = resource;
            SetState(State.Work);
            SetWorkState(WorkState.Gathering);

            RequestPath(resource.gridTile.position, resource.worldPosition);
        }
    }

    void TargetStore(ResourceBuilding store)
    {
        if (store != null)
        {
            if (storage.IsEmpty(store.type))
            {
                // If we don't have resources, just start gathering closest nodes
                ResourceNode closestResource = ResourceHandler.Instance.GetClosestNode(store);
                TargetResource(closestResource);
            }
            else
            {
                // Store resources if have some
                targetStore = store;
                SetState(State.Work);
                SetWorkState(WorkState.Storing);

                Vector2Int storePos = new Vector2Int((int)store.transform.position.x, (int)store.transform.position.z);

                RequestPath(storePos, store.transform.position);
                targetResource = null; // Don't return to gathering if we've commanded to store
            }
        }
    }

    void TargetBuilding(Building building)
    {
        if (building != null)
        {
            Debug.Log("Targetting building");
            targetBuilding = building;
            SetState(State.Work);
            SetWorkState(WorkState.Building);

            Vector2Int buildingPos = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.z);

            RequestPath(buildingPos, building.transform.position);
        }
    }
    #endregion

    #region Worker Actions (Gathering, storing and building)
    public void Gather()
    {
        if (!CheckCapacity())
        {
            Debug.Log("Gathering " + targetResource.type.ToString());

            storage.Add(targetResource.type, targetResource.Gather(5));
            interactTimer = interactInterval;

            if (targetResource.IsEmpty())
            {
                ResourceNode neighbuoringNode = ResourceHandler.Instance.GetClosestNeighbour(targetResource);

                if (neighbuoringNode != null)
                {
                    TargetResource(neighbuoringNode);
                }
                else
                {
                    targetResource = null;
                }    
            }

            CheckCapacity();
        }
    }

    // Checks if resources are at capacity and switch to storing if so
    bool CheckCapacity()
    {
        if (storage.AtCapacity())
        {
            // Find closest resource store
            ResourceBuilding closestStore = BuildingHandler.Instance.GetClosestStore(ResourceNode.Type.Wood, GridPos());
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

    bool Store()
    {
        if (targetStore != null)
        {
            targetStore.Store(storage.Get(targetStore.type));
            storage.Clear(targetStore.type);

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
