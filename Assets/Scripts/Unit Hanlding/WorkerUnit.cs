using UnityEngine;

public class WorkerUnit : FollowerUnit
{
    // YOU MUST ENSURE CONSISTENCY 0 = idle, 1 = moving, 2 = following
    public enum WorkerState
    {
        Idle,
        Moving,
        following, 
        Gathering,
        Storing
    }

    public WorkerState workerState;

    public int maxCapacity = 100;
    public int[] resourceCount = new int[(int)ResourceNode.Type.Max];


    ResourceNode targetResource;
    public ResourceStore targetStore;

    float gatherInterval = 0.5f, gatherTimer = 0;

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

        if (workerState == WorkerState.Moving)
        {
            FollowPath();
            handled = true;
        }
        else if (workerState == WorkerState.Gathering)
        {
            if (gatherTimer > 0) gatherTimer -= Time.deltaTime;

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
                    if (gatherTimer <= 0)
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

                    if (targetResource != null && !targetResource.IsEmpty())
                    {
                        // Continue gathering current resource
                        TargetResource(targetResource);
                    }
                    else if (targetResource != null)
                    {
                        // Find next resource
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


            if (building is ResourceStore)
            {
                ResourceStore store = (ResourceStore)building;

                if (store != null && CurrentCapacity() > 0)
                {
                    TargetStore(store);
                    targetResource = null; // Don't return to gathering if we've commanded to store
                }
            }

            // If building is broken, repair

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

        workerState = WorkerState.following;
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

    public void Gather()
    {

        if (!CheckCapacity())
        {
            Debug.Log("Gathering " + targetResource.type.ToString());

            resourceCount[(int)targetResource.type] += targetResource.Gather(5);
            gatherTimer = gatherInterval;

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
            ResourceStore closestStore = BuildingHandler.Instance.GetClosestStore(ResourceNode.Type.Tree, GridPos());
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
            Debug.Log("SHOULD STORE");
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
}
