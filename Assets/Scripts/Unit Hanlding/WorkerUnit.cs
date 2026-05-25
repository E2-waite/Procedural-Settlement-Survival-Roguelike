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

    public WorkerState workerState;

    public int maxCapacity = 100;
    public int[] resourceCount = new int[(int)ResourceNode.Type.Max];


    ResourceNode targetResource;
    public ResourceStore targetStore;
    Building targetBuilding;

    float interactInterval = 0.5f, interactTimer = 0;

    protected override void Start()
    {
        base.Start();
        workerState = WorkerState.Idle;
    }

    protected override int GetState()
    {
        return (int)workerState;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override bool HandleStates()
    {
        bool handled = false;

        if (interactTimer > 0) interactTimer -= Time.deltaTime;

        if (workerState == WorkerState.Moving)
        {
            FollowPath();
            handled = true;
        }
        else if (workerState == WorkerState.Gathering)
        {
            if (targetResource == null)
            {
                workerState = WorkerState.Idle;
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
        else if (workerState == WorkerState.Storing)
        {
            if (targetStore == null)
            {
                workerState = WorkerState.Idle;
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

                    // TODO: move to next resource type if available

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
        else if (workerState == WorkerState.Building)
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
                        workerState = WorkerState.Idle;
                    }
                }
            }
        }

        return handled;
    }

    public override void Command(GridTile tile)
    {
        if (tile == null) return;

        if (tile.HasResource())
        {
             // Command to gather if tile has resource
            ResourceNode resource = tile.GetResource();

            TargetResource(resource);
        }
        else if (tile.HasBuilding())
        {
            // Interact with building if tile has one
            Building building = tile.GetBuilding();




            // If building is broken, repair
            if (!building.Built())
            {
                TargetBuilding(building);
            }
            else
            {
                if (building is ResourceStore)
                {
                    ResourceStore store = (ResourceStore)building;

                    if (store != null && CurrentCapacity() > 0)
                    {
                        TargetStore(store);
                        targetResource = null; // Don't return to gathering if we've commanded to store
                    }
                }
            }

            // If building hasn't finished building, build

            // If building is repaired and built, interact

        }
        else
        {
            // Move to tile if empty
            Debug.Log("Commanding to move!");
            pathRequested = false;
            currentPath.Clear();
            workerState = WorkerState.Moving;


            RequestPath(GridPos(), tile.position, tile.worldPosition);
        }
    }

    Vector2Int GridPos()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));

    }

    public override void StartFollowing(PlayerController thePlayer)
    {
        base.StartFollowing(thePlayer);

        workerState = WorkerState.Following;
    }

    void TargetResource(ResourceNode resource)
    {
        if (resource != null && !resource.IsEmpty())
        {
            targetResource = resource;
            workerState = WorkerState.Gathering;

            RequestPath(GridPos(), resource.gridTile.position, resource.worldPosition);
        }
    }

    void TargetStore(ResourceStore store)
    {
        if (store != null)
        {
            targetStore = store;
            workerState = WorkerState.Storing;

            Vector2Int storePos = new Vector2Int((int)store.transform.position.x, (int)store.transform.position.z);

            RequestPath(GridPos(), storePos, store.transform.position);
        }
    }

    void TargetBuilding(Building building)
    {
        if (building != null)
        {
            targetBuilding = building;
            workerState = WorkerState.Building;

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
                workerState = WorkerState.Idle;
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
