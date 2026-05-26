using UnityEngine;

public class WorkerUnit : FollowerUnit
{
    // YOU MUST ENSURE CONSISTENCY 0 = idle, 1 = moving, 2 = following
    public enum WorkerState
    {
        Idle,
        Moving,
        Following, 
        Gathering,
        Storing,
        Building
    }

    public WorkerState state;
    WorkerState lastState;

    public int maxCapacity = 100;
    public int[] resourceCount = new int[(int)ResourceNode.Type.Max];


    ResourceNode targetResource;
    public ResourceStore targetStore;
    Building targetBuilding;

    float interactInterval = 0.5f, interactTimer = 0;

    protected override void Start()
    {
        base.Start();
        state = WorkerState.Idle;
        lastState = WorkerState.Idle;
    }

    protected override int GetState()
    {
        return (int)state;
    }

    protected override void SetState(int newState)
    {
        lastState = state;
        state = (WorkerState)newState;
    }

    void SetState(WorkerState newState)
    {
        lastState = state;
        state = newState;
    }


    protected override void Update()
    {
        base.Update();
    }



    protected override bool HandleStates()
    {
        bool handled = base.HandleStates();
        if (handled) return true;

        if (interactTimer > 0) interactTimer -= Time.deltaTime;

        if (state == WorkerState.Gathering)
        {
            if (targetResource == null)
            {
                SetState(WorkerState.Idle);
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
            handled = true;
        }
        else if (state == WorkerState.Storing)
        {
            if (targetStore == null)
            {
                SetState(WorkerState.Idle);
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
        else if (state == WorkerState.Building)
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
                        SetState(WorkerState.Idle);
                    }
                }
            }
        }

        return handled;
    }

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
                if (building is ResourceStore)
                {
                    TargetStore((ResourceStore)building);
                }
            }

            handled = true;
        }

        if (!handled)
            base.Command(tile);
    }

    public override void StartFollowing(PlayerController thePlayer)
    {
        base.StartFollowing(thePlayer);

        SetState(WorkerState.Following);
    }

    void TargetResource(ResourceNode resource)
    {
        if (resource != null && !resource.IsEmpty())
        {
            targetResource = resource;
            SetState(WorkerState.Gathering);

            RequestPath(GridPos(), resource.gridTile.position, resource.worldPosition);
        }
    }

    void TargetStore(ResourceStore store)
    {
        if (store != null)
        {
            if (resourceCount[(int)store.type] > 0)
            {
                // Store resources if have some
                targetStore = store;
                SetState(WorkerState.Storing);

                Vector2Int storePos = new Vector2Int((int)store.transform.position.x, (int)store.transform.position.z);

                RequestPath(GridPos(), storePos, store.transform.position);
                targetResource = null; // Don't return to gathering if we've commanded to store
            }
            else
            {
                // If we don't have resources, just start gathering closest nodes
                ResourceNode closestResource = ResourceHandler.Instance.GetClosestNode(store);

                TargetResource(closestResource);
            }
        }
    }

    void TargetBuilding(Building building)
    {
        if (building != null)
        {
            targetBuilding = building;
            SetState(WorkerState.Building);

            Vector2Int buildingPos = new Vector2Int((int)building.transform.position.x, (int)building.transform.position.z);

            RequestPath(GridPos(), buildingPos, building.transform.position);
        }
    }

    public void Gather()
    {

        if (!CheckCapacity())
        {
            Debug.Log("Gathering " + targetResource.type.ToString());

            resourceCount[(int)targetResource.type] += targetResource.Gather(5);
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
        if (CurrentCapacity() >= maxCapacity)
        {
            // Find closest resource store
            ResourceStore closestStore = BuildingHandler.Instance.GetClosestStore(ResourceNode.Type.Wood, GridPos());
            if (closestStore == null)
            {
                SetState(WorkerState.Idle);
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
            targetStore.Store(resourceCount[(int)targetStore.type]);

            resourceCount[(int)targetStore.type] = 0;

            return true;
        }
        return false;
    }

    int CurrentCapacity()
    {
        int total = 0;
        for (int i = 0; i < (int)ResourceNode.Type.Max; i++)
        {
            total += resourceCount[i];
        }
        return total;
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
}
